using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Прокси для перехвата методов на основе <see cref="DispatchProxy"/>.
    /// </summary>
    public class InterceptDispatchProxy : DispatchProxy
    {
        private IInterceptor _interceptor;

        private Func<MethodInfo, Func<object, object[], object>> _methodInvokerFactory;

        /// <summary>
        /// Исходный объект, вызовы которого будут перехватываться.
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Прокси.
        /// </summary>
        public object TransparentProxy { get; private set; }
        
        /// <summary>
        /// Исходный тип экземпляра (обычно - интерфейс, от которого строится прокси).
        /// </summary>
        public Type InstanceDeclaringType { get; private set; }

        /// <summary>
        /// Создает экземпляр прокси объекта типа <paramref name="instanceDeclaringType"/>.
        /// При этом тип обязательно должен быть интерфейсом.
        /// Все обращения к методам объекта <paramref name="instance"/>, которые реализуются от интерфейса
        /// <paramref name="instanceDeclaringType"/> будут проксированы через метод <see cref="Invoke(MethodInfo, object[])"/>
        /// и перехвачены обработчиком <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="instance">Экземпляр объекта для создания прокси.</param>
        /// <param name="instanceDeclaringType">Тип прокси. Задается как интерфейс, который реализуется типом объекта <paramref name="instance"/>.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        /// <returns>Экземляр прокси типа <paramref name="instanceDeclaringType"/>.</returns>
        public static object CreateTransparentProxy(object instance, Type instanceDeclaringType, IInterceptor interceptor)
        {
            if (!instanceDeclaringType.IsInterface)
            {
                throw new InvalidOperationException(
                    "Проксирование допускается только для интерфейсов. " +
                    $"Параметр '{nameof(instanceDeclaringType)}': {instanceDeclaringType.FullName}");
            }
            
            var transparentProxy = CreateTrasparentProxy<InterceptDispatchProxy>(instanceDeclaringType);
            var interceptProxy = (InterceptDispatchProxy) transparentProxy;
            
            interceptProxy.Initialize(instance, transparentProxy, instanceDeclaringType, interceptor);

            return transparentProxy;
        }

        /// <inheritdoc />
        protected internal override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                return AspectWeaver.NotInterceptableMethods.Any(m => ReferenceEquals(m, targetMethod)) 
                    ? Bypass(targetMethod, args)
                    : Intercept(targetMethod, args);
            }
            catch (TargetInvocationException e)
            {
                e.Unwrap().Rethrow();
            }

            return default;
        }

        /// <summary>
        /// Выполняет вызов метода без перехвата.
        /// </summary>
        /// <param name="targetMethod">Исходный метод.</param>
        /// <param name="args">Аргументы метода.</param>
        /// <returns>Результат выполнения метода (null, если тип void).</returns>
        private object Bypass(MethodInfo targetMethod, object[] args)
        {
            return targetMethod.Invoke(Instance, args);
        }

        /// <summary>
        /// Выполняет перехват вызова метода.
        /// </summary>
        /// <param name="targetMethod">Исходный метод.</param>
        /// <param name="args">Аргументы метода.</param>
        /// <returns>Результат выполнения метода (null, если тип void).</returns>
        private object Intercept(MethodInfo targetMethod, object[] args)
        {
            var context = new InvocationContext(args, targetMethod, Instance, TransparentProxy, InstanceDeclaringType);
            var invoker = _methodInvokerFactory(targetMethod);
            
            var invocation = new Invocation(context, invoker);
            
            _interceptor.Intercept(invocation);

            return targetMethod.IsVoidReturn()
                ? default
                : invocation.Context.ReturnValue;
        }

        /// <summary>
        /// Выполняет инициализацию прокси.
        /// </summary>
        /// <param name="instance">Экземпляр объекта для создания прокси.</param>
        /// <param name="transparentProxy">Прокси.</param>
        /// <param name="instanceDeclaringType">Тип прокси. Задается как интерфейс, который реализуется типом объекта <paramref name="instance"/>.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        private void Initialize(object instance, object transparentProxy, Type instanceDeclaringType, IInterceptor interceptor)
        {
            Instance = instance;
            InstanceDeclaringType = instanceDeclaringType;
            TransparentProxy = transparentProxy;
            _interceptor = interceptor;

            _methodInvokerFactory = Memoizer.Memoize<MethodInfo, Func<object, object[], object>>(
                Expressions.CreateMethodInvoker);
        }
    }
}