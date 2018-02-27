
namespace IvoryProxy.Core.Proxies
{
    #if NETCOREAPP2_0
      
    using System;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using Exceptions;
    using Providers;

    /// <summary>
    /// Реализация динамического прокси на основе <see cref="DispatchProxy"/>.
    /// </summary>
    /// <typeparam name="T">Тип объекта для проксирования.</typeparam>
    public class IvoryDispatchProxy<T> : DispatchProxy, IIvoryProxy<T> 
        where T : class 
    {
        private IvoryInterceptorProxyBase<T> _interceptorProxyBase;

        /// <inheritdoc />
        public T Decorated => _interceptorProxyBase.Decorated;

        /// <inheritdoc />
        public T TransparentProxy => _interceptorProxyBase.TransparentProxy;

        /// <summary>
        /// Создает прокси объекта.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        /// <returns>Экземпляр прокси.</returns>
        /// <exception cref="IvoryProxyException">В случае, если тип <typeparamref name="T"/> не является интерфейсом.</exception>
        /// <exception cref="ArgumentNullException">В случае, если <paramref name="decorated"/> равен <c>null</c>.</exception>
        public static IvoryDispatchProxy<T> CreateProxy(T decorated)
        {  
            if (decorated == null)
                throw new ArgumentNullException(nameof(decorated));
            
            object proxy = Create<T, IvoryDispatchProxy<T>>();
            
            var typedProxy = (IvoryDispatchProxy<T>) proxy;        
            typedProxy.Initialize(decorated, (T)proxy, new AttributeInterceptorProvider());
            
            return typedProxy;
        }
        
        /// <inheritdoc />
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                var invocation = new Invocation(Decorated, args, targetMethod, typeof(T));
                _interceptorProxyBase.Proxy(invocation);

                return invocation.ReturnValue;
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
            
            // Dummy: никогда не вызовется
            return default;
        }

        /// <summary>
        /// Выполняет инициализацию прокси объекта.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        /// <param name="proxy">Проксированный объект.</param>
        /// <param name="interceptorProvider">Провайдер перехватчиков вызовов методов.</param>
        protected virtual void Initialize(T decorated, T proxy, IInterceptorProvider interceptorProvider)
        {
            _interceptorProxyBase = new IvoryInterceptorProxyBase<T>(decorated, proxy, interceptorProvider);
        }
    }
    
    #endif
}