using System;
using System.Collections.Generic;
using IvorySharp.Core;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Пайплайн выполнения вызова метода.
    /// </summary>
    public interface IInvocationPipeline
    {
        /// <summary>
        /// Провайдер сервисов.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
        
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
        FlowBehaviour FlowBehaviour { get; }
        
        /// <summary>
        /// Признак возможности вернуть результат из метода.
        /// По нему можно определить какой метод возврата (<see cref="Return()"/> или <see cref="ReturnValue(object)"/>)
        /// можно вызывать в текущем контексте.
        /// </summary>
        bool CanReturnResult { get; }
        
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
        /// Прекращает выполнение пайплайна с результатом по умолчанию.
        /// Метод затрет текущее значение результата в <see cref="InvocationContext.ReturnValue"/>
        /// свойства <see cref="Context"/> на значение по умолчанию для типа возвращаемого методом результата.
        /// </summary>
        void ReturnDefault();

        /// <summary>
        /// Прекращает выполнение пайплайна без изменения результата, заданного на момент вызова.
        /// Если на момент вызова, значение возвращаемого результата не было установлено, то поведение будет
        /// аналогичным вызову <see cref="ReturnDefault()"/>.
        /// </summary>
        void Return();
        
        /// <summary>
        /// Завершает выполнение пайплайна, выбрасывая новое исключение.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        void ThrowException(Exception exception);
    }
}