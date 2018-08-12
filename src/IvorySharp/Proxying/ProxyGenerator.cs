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
    /// Усовершенствованная версия генератора DispatchProxy.
    /// </summary>
    internal class ProxyGenerator
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
                packed.ReturnValue = FastProxyInvoke(packed.Proxy, new object[] { method, packed.MethodArguments });
            }
            catch (TargetInvocationException tie)
            {
                tie.GetInner().Throw();
            }
        }
    }
}