namespace IvoryProxy.Core
{
    /// <summary>
    /// Провайдер перехватчиков вызова метода.
    /// </summary>
    public interface IInterceptorSelector
    {
        /// <summary>
        /// Получает экземпляр первого обработчика вызова метода по сигнатуре вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Перехватчик вызовов методов.</returns>
        IInterceptor FirstOrDefaultInterceptor(IMethodInvocation invocation);
    }
}