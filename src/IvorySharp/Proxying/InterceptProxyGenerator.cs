using System;
using IvorySharp.Core;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Генератор прокси для перехватов вызова методов объекта.
    /// </summary>
    public sealed class InterceptProxyGenerator : IInterceptProxyGenerator
    {
        /// <summary>
        /// Экземпляр генератора по умолчанию.
        /// </summary>
        public static InterceptProxyGenerator Default { get; } = new InterceptProxyGenerator();

        private InterceptProxyGenerator() { }

        /// <inheritdoc />
        public T CreateInterceptProxy<T>(T target, IInterceptor interceptor) where T : class
        {
            return (T)InterceptDispatchProxy.CreateTransparentProxy(target, typeof(T), interceptor);
        }

        /// <inheritdoc />
        public object CreateInterceptProxy(object target, Type targetDeclaredType, IInterceptor interceptor)
        {
            return InterceptDispatchProxy.CreateTransparentProxy(target, targetDeclaredType, interceptor);
        }
    }
}