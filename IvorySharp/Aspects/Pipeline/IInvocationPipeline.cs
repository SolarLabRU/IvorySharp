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
        InvocationContext InvocationContext { get; }

        /// <summary>
        /// Текущее исключение.
        /// </summary>
        Exception CurrentException { get; }
        
        /// <summary>
        /// Текущее состояние потока.
        /// </summary>
        FlowBehaviour FlowBehaviour { get; }

        /// <summary>
        /// Прекращает выполнение пайплайна.
        /// </summary>
        /// <param name="returnValue">Значение возврата.</param>
        void Return(object returnValue);

        /// <summary>
        /// Прекращает выполнение пайплайна.
        /// </summary>
        void Return(); 
        
        /// <summary>
        /// Завершает выполнение пайплайна, выбрасывая новое исключение.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void ThrowException(Exception exception);
    }
}