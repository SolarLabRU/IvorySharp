namespace IvorySharp.Core
{
    /// <summary>
    /// Интерфейс компонента для перехвата выполнения методов.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Перехватывает выполнение метода.
        /// Обычно в конце вызывает <see cref="IInvocation.Proceed()"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода</param>
        void Intercept(IInvocation invocation);
    }
}