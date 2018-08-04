using System;
using IvorySharp.Core;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Генератор прокси для перехватов вызова методов объекта.
    /// </summary>
    internal sealed class InterceptProxyGenerator : IInterceptProxyGenerator
    {
        /// <summary>
        /// Экземпляр генератора по умолчанию.
        /// </summary>
        public static InterceptProxyGenerator Default { get; } = new InterceptProxyGenerator();

        private InterceptProxyGenerator() { }

        /// <inheritdoc />
        public TInterface CreateInterceptProxy<TInterface, TClass>(TClass target, IInterceptor interceptor) 
            where TInterface : class 
            where TClass : TInterface
        {
            return (TInterface) InterceptDispatchProxy.CreateTransparentProxy(target, typeof(TInterface), typeof(TClass), interceptor);
        }

        /// <inheritdoc />
        public object CreateInterceptProxy(object target, Type declaredType, Type targetType, IInterceptor interceptor)
        {
            return InterceptDispatchProxy.CreateTransparentProxy(target, declaredType, targetType, interceptor);
        }
    }
}