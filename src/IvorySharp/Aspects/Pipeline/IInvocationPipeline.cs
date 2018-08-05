using System;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Пайплайн выполнения вызова метода.
    /// </summary>
    public interface IInvocationPipeline
    {
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        InvocationContext Context { get; }

        /// <summary>
        /// Текущее исключение.
        /// </summary>
        Exception CurrentException { get; }
        
        /// <summary>
        /// Текущее состояние потока.
        /// </summary>
        FlowBehavior FlowBehavior { get; }
        
        /// <summary>
        /// Пользовательское состояние, устанавливаемое в рамках выполнения текущего аспекта.
        /// </summary>
        object AspectExecutionState { get; set; }
        
        /// <summary>
        /// Прекращает выполнение пайплайна, возвращая результат.
        /// Вся ответственность за приведение типов лежит на стороне клиентского кода.
        /// Если приведение типов невозможно, то будет выброшено исключение.
        /// </summary>
        /// <param name="returnValue">Значение возврата.</param>
        void ReturnValue(object returnValue);

        /// <summary>
        /// Прекращает выполнение пайплайна без изменения результата, заданного на момент вызова.
        /// </summary>
        void Return();
        
        /// <summary>
        /// Завершает выполнение пайплайна, выбрасывая новое исключение.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void ThrowException(Exception exception);

        /// <summary>
        /// Прокидывает исключение дальше по пайплайну, не прерывая выполнение.
        /// </summary>
        /// <param name="exception">Исключение,</param>
        void RethrowException(Exception exception);
    }
}