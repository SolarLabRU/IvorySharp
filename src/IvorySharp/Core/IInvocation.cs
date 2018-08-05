namespace IvorySharp.Core
{
    /// <summary>
    /// Модель выполнения метода.
    /// </summary>
    public interface IInvocation
    {
        /// <summary>
        /// Контекст выполнения метода.
        /// </summary>
        InvocationContext Context { get; }
        
        /// <summary>
        /// Выполняет оригинальный метод.
        /// </summary>
        /// <returns>Результат вызова метода (void -> null).</returns>
        object Proceed();
    }
}