namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Перехватчик, который вызывает запрошенный метод без изменений.
    /// </summary>
    internal class BypassMethodInterceptor : IMethodInterceptor
    {
        /// <summary>
        /// Экземпляр перехватчика.
        /// </summary>
        public static BypassMethodInterceptor Instance { get; } = new BypassMethodInterceptor();
        
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}