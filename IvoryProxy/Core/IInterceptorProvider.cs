namespace IvoryProxy.Core
{
    /// <summary>
    /// Провайдер перехватчиков вызова метода.
    /// </summary>
    public interface IInterceptorProvider
    {
        /// <summary>
        /// Получает экземпляр обработчика вызова метода по сигнатуре вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Перехватчик вызовов методов.</returns>
        IInterceptor GetInterceptor(IMethodInvocation invocation);
    }
}