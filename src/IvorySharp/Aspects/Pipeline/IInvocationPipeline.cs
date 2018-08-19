using System;
using System.ComponentModel;
using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Пайплайн выполнения вызова метода.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInvocationPipeline
    {
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        [NotNull] IInvocationContext Context { get; }

        /// <summary>
        /// Исключение, возникшее в ходе выполнения пайплайна.
        /// </summary>
        Exception CurrentException { get; }
        
        /// <summary>
        /// Возвращаемое значение, установленное в ходе выполнения пайплайна.
        /// </summary>
        object CurrentReturnValue { get; }
        
        /// <summary>
        /// Текущее состояние потока.
        /// </summary>
        FlowBehavior FlowBehavior { get; }
        
        /// <summary>
        /// Пользовательское состояние, устанавливаемое в рамках выполнения текущего аспекта.
        /// </summary>
        [NotNull] object ExecutionState { get; set; }
        
        /// <summary>
        /// Выполняет возврат значения из пайплайна.
        /// Устанавливает состояние пайплайна в <see cref="Aspects.Pipeline.FlowBehavior.Return"/>.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        void ReturnValue([CanBeNull] object returnValue);

        /// <summary>
        /// Выполняет возврат значения по умолчанию (default({ReturnType})) из пайплайна.
        /// Устанавливает состояние пайплайна в <see cref="Aspects.Pipeline.FlowBehavior.Return"/>.
        /// </summary>
        void Return();
        
        /// <summary>
        /// Завершает выполнение пайплайна, выбрасывая новое исключение.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void ThrowException([NotNull] Exception exception);

        /// <summary>
        /// Прокидывает исключение дальше по пайплайну, не прерывая выполнение.
        /// </summary>
        /// <param name="exception">Исключение,</param>
        void RethrowException([NotNull] Exception exception);
    }
}