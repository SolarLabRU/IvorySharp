using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Providers
{
    /// <summary>
    /// Провайдер перехватчиков вызова метода.
    /// </summary>
    public interface IMethodInterceptorProvider
    {
        /// <summary>
        /// Получает экземпляр обработчика вызова метода по сигнатуре вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Перехватчик вызовов методов.</returns>
        IMethodInterceptor GetInterceptor(IMethodInvocation invocation);
    }
}