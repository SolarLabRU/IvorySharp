using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Providers
{
    /// <summary>
    /// Провайдер перехватчиков вызова метода.
    /// </summary>
    public interface IInterceptorProvider
    {
        /// <summary>
        /// Возвращает перехватчик для вызова метода <paramref name="invocation"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Экземпляр перехватчика вызова метода.</returns>
        IInterceptor GetInterceptor(IInvocation invocation);
    }
}