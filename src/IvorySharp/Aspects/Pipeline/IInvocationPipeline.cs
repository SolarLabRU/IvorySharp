using System;
using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Пайплайн выполнения вызова метода.
    /// </summary>
    [PublicAPI]
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
        /// Выполняет возврат значения из пайплайна, прекращая его выполнение.
        /// Устанавливает состояние пайплайна в <see cref="Aspects.Pipeline.FlowBehavior.Return"/>.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        void Return([CanBeNull] object returnValue);

        /// <summary>
        /// Выполняет возврат значения по умолчанию (default({ReturnType})) из пайплайна, при этом
        /// прекращая его выполнение.
        /// Устанавливает состояние пайплайна в <see cref="Aspects.Pipeline.FlowBehavior.Return"/>.
        /// </summary>
        void Return();
        
        /// <summary>
        /// Прекращает выполнение пайплайна, выбрасывая новое исключение.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void Throw([NotNull] Exception exception);

        /// <summary>
        /// Продолжает выполнение пайплайна, устанавливая исключение.
        /// Переводит состояние пайплайна в <see cref="Aspects.Pipeline.FlowBehavior.RethrowException"/>.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void ContinueFaulted([NotNull] Exception exception);

        /// <summary>
        /// Продолжает выполнение пайплайна, устанавливая возвращаемый результат.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        void Continue([CanBeNull] object returnValue);

        /// <summary>
        /// Продолжает выполнение пайплайна, устанавливая возвращаемый результат по умолчанию.
        /// </summary>
        void Continue();
    }
}