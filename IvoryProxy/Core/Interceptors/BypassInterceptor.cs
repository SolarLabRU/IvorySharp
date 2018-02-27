using System;

namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Перехватчик, который передает управление в исходный метод без выполнения проксирования.
    /// </summary>
    internal class BypassInterceptor : IInterceptor
    {
        /// <inheritdoc />
        public string InterceptorKey { get; }

        public BypassInterceptor()
        {
            InterceptorKey = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}