namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Компонент, выполняющий пехеват вызова методов целевого объекта.
    /// </summary>
    public interface IMethodInterceptor
    {
        /// <summary>
        /// Выполняет перехват вызова метода <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        void Intercept(IMethodInvocation invocation);
    }
}