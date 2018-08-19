using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Генератор экземпляров прокси.
    /// Усовершенствованная версия генератора DispatchProxy:
    ///   1) Улучшен кеш типов и способ передачи токенов
    ///   2) Добавлен кеш для быстрого вызова методов
    ///
    /// Аналогично DispatchProxy, экземпляры сгенерированных прокси
    /// не кешируются. Кешируются только динамические типы.
    /// 
    /// </summary>
    ///
    /// <remarks>
    ///   Иерархия:
    ///
    ///     // Интерфейс сервиса, вызовы которого необходимо перехватывать
    ///     interface IService { }
    ///
    ///     // Прокси класс, которому будут транслироваться вызовы исходного сервиса.
    ///     class YourProxy : IvoryProxy { }
    ///
    ///     // Класс, который генерируется библиотекой в рантайме
    ///     dynclass Proxy : YourProxy, IService { }
    /// 
    /// </remarks>
    ///
    /// <![CDATA[
    /// 
    ///   Что сгенерируется в рантайме.
    ///
    ///   Ключевые слова:
    ///   1) [captured: ...]
    ///       переменные захвачены из контекста генератора и в сгененированном динамическом классе их нет
    ///   2) [conditional emit: ...]
    ///       условная генерация кода по условию из внешнего контекста
    /// 
    ///   interface IService {
    ///        int Identity(int arg);
    ///   }
    ///
    ///   dynclass DynamicProxy : IvoryProxy, IService {
    /// 
    ///       private Action{object[]} invoke;
    ///
    ///       public DynProxy(Action{object[]} translator)
    ///           : base()
    ///       {
    ///           this.invoke = translator;
    ///       }
    ///
    ///       public Identity(int arg)
    ///       {
    ///           [captured:
    ///              var $method= typeof(IService.Identity);
    ///              var $parameters = GetMethod(IService.Identity).GetParameters();
    ///              var $parameterTypes = parameters.Select(p => p.Type);
    ///           ]
    /// 
    ///           var packedArgs = new object[PackedArguments.Count];
    ///           packedArgs[PackedArgPosition.Proxy] = this;
    ///
    ///           [captured: 
    ///               var $token = MethodLinkStore.CreateToken($method);
    ///           ]
    ///
    ///           packedArgs[PackedArgPosition.DeclaringType] = $token.DeclaringType;
    ///           packedArgs[PackedArgPosition.MethodTokenKey] = $token.Key;
    /// 
    ///           var args = new object[$parameters.Length];
    ///           for (int i = 0; i < args.Length; i++)
    ///               args[i] = cast<$parameterTypes[i]>($parameters[i].Value);
    ///
    ///           packedArgs[PackedArgPosition.MethodArguments] = args;
    ///
    ///           [conditional emit:
    ///               if ($method.ContainsGenericParameters) {
    ///                   [captured:
    ///                       var $genericArgs = $method.GetGenericArguments();
    ///                   ]
    /// 
    ///                   var genArgs = new Type[genericArgs.Length];
    ///                   for(int i = 0; i < genArgs.Length; i++)
    ///                       genArgs[i] = $genericArgs[i];
    ///               }
    ///           ]
    ///
    ///           invoke(packedArgs);
    /// 
    ///           [conditional emit:
    ///               if ($method.ReturnType != typeof(void))
    ///                   return packedArgs[PackedArgPosition.ReturnValue];
    ///               else
    ///                   return;
    ///           ]
    ///       }
    ///   }
    ///
    /// ]]>
    internal sealed class ProxyGenerator
    {
        private static readonly object Lock = new object();

        private static readonly Func<object, object[], object> FastProxyInvoke =
            Expressions.CreateLambda(MethodReferences.ProxyInvoke);

        /// <summary>
        /// Хранит информацию о связях методов.
        /// </summary>
        private static readonly MethodLinkStore MethodLinkStore = new MethodLinkStore();

        /// <summary>
        /// Инициализированный экземпляр <see cref="ProxyGenerator"/>.
        /// </summary>
        public static readonly ProxyGenerator Instance = new ProxyGenerator();

        /// <summary>
        /// Кеш прокси типов.
        /// </summary>
        private readonly ProxyTypeCache _proxyTypeCache;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ProxyGenerator"/>.
        /// </summary>
        private ProxyGenerator()
        {
            var proxyAssembly = new ProxyAssembly(prefix: "DynamicIvorySharp", MethodLinkStore);
            _proxyTypeCache = new ProxyTypeCache(new ProxyTypeGeneratorFacade(proxyAssembly));
        }

        /// <summary>
        /// Создает экземпляр прокси.
        /// </summary>
        /// <param name="baseType">Базовый тип.</param>
        /// <param name="interfaceType">Тип интерфейса.</param>
        /// <returns>Экземпляр прокси.</returns>
        public object CreateTransparentProxy(Type baseType, Type interfaceType)
        {
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(interfaceType != null, "interfaceType != null");

            lock (Lock)
            {
                var proxyType = _proxyTypeCache.GetProxyType(baseType, interfaceType);
                return Activator.CreateInstance(proxyType, (Action<object[]>) TranslateInvoke);
            }
        }

        /// <summary>
        /// Вспомогательный метод для трансирования перехватываемых вызовов в
        /// метод <see cref="IvoryProxy.Invoke(MethodInfo, object[])"/>.
        /// Этот метод вызывается всеми сгенерированными прокси.
        /// Его задача распаковать аргументы и указатель на *this* и передать его на
        /// вход целевому методу прокси <see cref="MethodReferences.ProxyInvoke"/>.
        /// </summary>
        /// <param name="packedArgs">Упакованные параметры вызова.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TranslateInvoke(object[] packedArgs)
        {
            var packed = new PackedArguments(packedArgs);
            var method = MethodLinkStore.ResolveMethod(packed.MethodToken);

            if (method.IsGenericMethodDefinition && method is MethodInfo mi)
                method = mi.MakeGenericMethod(packed.GenericTypes);

            try
            {
                packed.ReturnValue = FastProxyInvoke(packed.Proxy, new object[] {method, packed.MethodArguments});
            }
            catch (TargetInvocationException tie)
            {
                tie.GetInner().Throw();
            }
        }
    }
}