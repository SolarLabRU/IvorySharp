namespace IvoryProxy.Core.Proxies
{
#if NET461

    using System;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using System.Reflection;
    using Providers;

    /// <summary>
    /// Реализация динамического прокси на основе <see cref="RealProxy"/>.
    /// </summary>
    /// <typeparam name="T">Тип объекта для проксирования.</typeparam>
    public class IvoryRealProxy<T> : RealProxy, IIvoryProxy<T>
        where T : class
    {
        private readonly IvoryInterceptorProxyBase<T> _interceptorProxyBase;

        /// <inheritdoc />
        public T Decorated => _interceptorProxyBase.Decorated;

        /// <inheritdoc />
        public T TransparentProxy => _interceptorProxyBase.TransparentProxy;

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvoryRealProxy{T}"/>.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        internal IvoryRealProxy(T decorated)
            : this(decorated, new AttributeBasedInterceptorProvider())
        { }

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvoryRealProxy{T}"/>.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        /// <param name="interceptorProvider">Провайдер перехватчиков.</param>
        internal IvoryRealProxy(T decorated, IInterceptorProvider interceptorProvider)
            : base(typeof(T))
        {
            if (decorated == null)
                throw new ArgumentNullException(nameof(decorated));

            if (interceptorProvider == null)
                throw new ArgumentNullException(nameof(interceptorProvider));

            _interceptorProxyBase = new IvoryInterceptorProxyBase<T>(decorated, (T)GetTransparentProxy(), interceptorProvider);
        }

        /// <inheritdoc />
        public override IMessage Invoke(IMessage msg)
        {
            if (msg is IMethodCallMessage mcm)
            {
                var invocation = new MethodInvocation(Decorated, mcm.Args, (MethodInfo)mcm.MethodBase, typeof(T));

                _interceptorProxyBase.Proxy(invocation);

                return new ReturnMessage(invocation.ReturnValue, invocation.Arguments, invocation.Arguments.Length, null, mcm);
            }

            return msg;
        }
    }

#endif
}