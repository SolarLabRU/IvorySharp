namespace IvorySharp.Core
{
    /// <summary>
    /// Интерфейс выполнения метода.
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
        
        /// <summary>
        /// Устанавливает возвращаемое значение вызова.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        void SetReturnValue(object returnValue);
    }
}