namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Компонент, выполняющий пехеват вызова методов целевого объекта.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Уникальный ключ перехватчика вызовов.
        /// </summary>
        string InterceptorKey { get; }
        
        /// <summary>
        /// Выполняет перехват вызова метода <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        void Intercept(IInvocation invocation);
    }
}