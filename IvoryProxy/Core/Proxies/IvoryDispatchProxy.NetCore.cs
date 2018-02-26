using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Core.Interceptors;
using IvoryProxy.Core.Providers;

namespace IvoryProxy.Core.Proxies
{
    #if NETCOREAPP2_0
      
    /// <summary>
    /// Реализация динамического прокси на основе <see cref="DispatchProxy"/>.
    /// </summary>
    /// <typeparam name="T">Тип объекта для проксирования.</typeparam>
    public class IvoryDispatchProxy<T> : DispatchProxy, IIvoryProxy<T> 
        where T : class 
    {
        /// <summary>
        /// Провайдер перехватчиков.
        /// </summary>
        protected IMethodInterceptorProvider MethodInterceptorProvider { get; set; }

        /// <inheritdoc />
        public T Decorated { get; protected set; }

        /// <inheritdoc />
        public T Proxy { get; protected set; }

        /// <summary>
        /// Создает прокси объекта.
        /// </summary>
        /// <param name="decorated">Исходный объект.</param>
        /// <returns></returns>
        /// <exception cref="IvoryProxyException">В случае, если тип <typeparamref name="T"/> не является интерфейсом.</exception>
        /// <exception cref="ArgumentNullException">В случае, если <paramref name="decorated"/> равен <c>null</c>.</exception>
        public static IvoryDispatchProxy<T> CreateProxy(T decorated)
        {
            if (!typeof(T).IsInterface)
            {
                throw new IvoryProxyException(
                    $"Проксирование возможно только для интерфейсов. Тип '{typeof(T)}' не является интерейсвом");
            }
            
            if (decorated == null)
                throw new ArgumentNullException(nameof(decorated));
            
            object proxy = Create<T, IvoryDispatchProxy<T>>();
            
            var typedProxy = (IvoryDispatchProxy<T>) proxy;        
            typedProxy.Initialize((T)proxy, decorated, new AttributeBasedMethodInterceptorProvider());
            
            return typedProxy;
        }
        
        /// <inheritdoc />
        public void Proceed(IMethodInvocation invocation)
        {
            Invoke(invocation.TargetMethod, invocation.Arguments);
        }

        /// <inheritdoc />
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                var invocation = new MethodInvocation(Decorated, args, targetMethod);
                var interceptor = MethodInterceptorProvider.GetInterceptor(invocation);

                if (interceptor != null)
                {
                    interceptor.Intercept(invocation);
                }
                else
                {
                    BypassMethodInterceptor.Instance.Intercept(invocation);
                    return invocation.ReturnValue;
                }

                if (invocation.IsReturnVoid)
                    return default(object);

                if (!invocation.IsReturnValueWasSet)
                {
                    throw new IvoryProxyException(
                        $"Не было установлено значение проксированного метода '{invocation.TargetMethod.Name}' " +
                        $"класса '{invocation.Target.GetType().FullName}'. " +
                        $"Проверьте, что в методе '{nameof(IMethodInterceptor.Intercept)}' " +
                        $"перехватчика '{interceptor.GetType().FullName}' вызывается метод " +
                        $"'{nameof(IMethodInvocation.TrySetReturnValue)}' параметра с типом '{nameof(IMethodInvocation)}'. " +
                        $"Если полная замена метода не была задумана, то убедитесь в наличии " +
                        $"вызова метода '{nameof(IMethodInvocation.Proceed)}' входного параметра.");
                }

                return !invocation.IsReturnVoid 
                    ? invocation.ReturnValue
                    : default(object);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
            
            // Dummy: никогда не вызовется
            return default(object);
        }

        /// <summary>
        /// Выполняет инициализацию прокси объекта.
        /// </summary>
        /// <param name="proxy">Проксированный объект.</param>
        /// <param name="source">Исходный объект.</param>
        /// <param name="interceptorProvider">Провайдер перехватчиков вызовов методов.</param>
        protected virtual void Initialize(T proxy, T source,
            IMethodInterceptorProvider interceptorProvider)
        {
            MethodInterceptorProvider = interceptorProvider ?? throw new ArgumentNullException(nameof(interceptorProvider));
            Decorated = source;
            Proxy = proxy;
        }
    }
    
    #endif
}