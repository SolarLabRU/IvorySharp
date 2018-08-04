using System;
using System.Reflection;
using IvorySharp.Aspects.Components.Caching;
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

        private Func<MethodInfo, Func<object, object[], object>> _cachedMethodInvokerFactory;

        /// <summary>
        /// Исходный объект, вызовы которого будут перехватываться.
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        public object TransparentProxy { get; private set; }

        /// <summary>
        /// Тип, в котором объявлен целевой метод (интерфейс).
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Тип, в котором содержится реализация целевого метода.
        /// </summary>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Создает экземпляр прокси объекта типа <paramref name="declaringType"/>.
        /// При этом тип обязательно должен быть интерфейсом.
        /// Все обращения к методам объекта <paramref name="instance"/>, которые реализуются от интерфейса
        /// <paramref name="declaringType"/> будут проксированы через метод <see cref="Invoke(MethodInfo, object[])"/>
        /// и перехвачены обработчиком <paramref name="interceptor"/>.
        /// </summary>
        /// <param name="instance">Экземпляр объекта для создания прокси.</param>
        /// <param name="declaringType">Объявленный тип экземпляра (интерфейс).</param>
        /// <param name="targetType">Фактический тип экземпляра.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        /// <returns>Экземляр прокси типа <paramref name="declaringType"/>.</returns>
        public static object CreateTransparentProxy(object instance, Type declaringType, Type targetType, IInterceptor interceptor)
        {
            if (!declaringType.IsInterface)
            {
                throw new InvalidOperationException(
                    "Проксирование допускается только для интерфейсов. " +
                    $"Параметр '{nameof(declaringType)}': {declaringType.FullName}");
            }
            
            var transparentProxy = CreateTrasparentProxy<InterceptDispatchProxy>(declaringType);
            var interceptProxy = (InterceptDispatchProxy) transparentProxy;
            
            interceptProxy.Initialize(instance, transparentProxy, declaringType, targetType, interceptor);

            return transparentProxy;
        }

        /// <inheritdoc />
        protected internal override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                return targetMethod.IsInterceptable()
                    ? Intercept(targetMethod, args) 
                    : Bypass(targetMethod, args);
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
            var context = new InvocationContext(args, targetMethod, Instance, TransparentProxy, DeclaringType, TargetType);
            var invoker = _cachedMethodInvokerFactory(targetMethod);
            
            var invocation = new Invocation(context, invoker);
            
            _interceptor.Intercept(invocation);

            return targetMethod.IsVoidReturn() ? default : invocation.Context.ReturnValue;
        }

        /// <summary>
        /// Выполняет инициализацию прокси.
        /// </summary>
        /// <param name="instance">Экземпляр класса для перехвата вызовов.</param>
        /// <param name="transparentProxy">Экземпляр прокси.</param>
        /// <param name="declaringType">Тип, в котором объявлен метод.</param>
        /// <param name="targetType">Тип, в котором содержится реализация метода.</param>
        /// <param name="interceptor">Компонент для перехвата вызовов методов.</param>
        private void Initialize(object instance, object transparentProxy, Type declaringType, Type targetType, IInterceptor interceptor)
        {
            Instance = instance;
            TransparentProxy = transparentProxy;
            DeclaringType = declaringType;
            TargetType = targetType;

            _interceptor = interceptor;

            _cachedMethodInvokerFactory = Cache.CreateProducer<MethodInfo, Func<object, object[], object>>(
                Expressions.CreateMethodInvoker);
        }
    }
}