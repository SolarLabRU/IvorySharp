using System;
using IvorySharp.Core;
using IvorySharp.Proxying.Platform.NetFramework;
#if NETCOREAPP2_0
using IvorySharp.Proxying.Platform.NetCore;
#elif NET461

#endif

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
#if NET461
            return (T)InterceptRealProxy.CreateTransparentProxy(target, typeof(T), interceptor);
#else
            return (T)InterceptDispatchProxy.CreateTransparentProxy(target, typeof(T), interceptor);
#endif
        }

        /// <inheritdoc />
        public object CreateInterceptProxy(object target, Type targetDeclaredType, IInterceptor interceptor)
        {
#if NET461
            return InterceptRealProxy.CreateTransparentProxy(target, targetDeclaredType, interceptor);
#else
            return InterceptDispatchProxy.CreateTransparentProxy(target, targetDeclaredType, interceptor);
#endif
        }
    }
}