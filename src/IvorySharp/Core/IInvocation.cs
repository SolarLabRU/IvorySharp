using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Интерфейс выполнения метода.
    /// </summary>
    [PublicAPI]
    public interface IInvocation
    {
        /// <summary>
        /// Контекст выполнения метода.
        /// </summary>
        InvocationContext Context { get; }
        
        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        object ReturnValue { get; set; }
    
        /// <summary>
        /// Выполняет оригинальный метод.
        /// </summary>
        /// <returns>Результат вызова метода (void -> null).</returns>
        [CanBeNull] object Proceed();
    }
}