using System;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Вспомогательный класс для работы с потоком выполнения пайплайна.
    /// </summary>
    internal static class InvocationPipelineFlow
    {
        /// <summary>
        /// Выполняет признак того, что в текущем состоянии пайплайна может быть выполнен обработчик с указанным именем.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения метода.</param>
        /// <param name="boundary">Имя обработчика, который запрашивает возможность выполнения.</param>
        /// <returns>Признак возможности выполнения.</returns>
        /// <exception cref="InvalidOperationException">Если имя обработчика неизвестно.</exception>
        internal static bool CanContinueBoundary(IInvocationPipeline pipeline, string boundary)
        {
            // Независимо от состояния в которое перешел пайплайн
            // точка входа в метод выполняется всегда.
            if (boundary == nameof(IMethodBoundaryAspect.OnEntry))
            {
                return pipeline.FlowBehaviour != FlowBehaviour.ThrowException;
            }

            // Исключения обрабатываем только если флоу по умолчанию или 
            // явно установлен Rethrow.
            if (boundary == nameof(IMethodBoundaryAspect.OnException))
            {
                return pipeline.FlowBehaviour == FlowBehaviour.Default ||
                       pipeline.FlowBehaviour == FlowBehaviour.RethrowException;

                // В случае PipelineFlowBehaviour.ThrowException исключение наверх должно прокидываться без обработки
            }

            // Независимо от состояния в которое перешел пайплайн
            // точка выхода из метода выполняется всегда.
            if (boundary == nameof(IMethodBoundaryAspect.OnExit))
            {
                return pipeline.FlowBehaviour != FlowBehaviour.ThrowException;
            }

            // Обработчик 'OnSuccess' вызывается если состояние Return или Default.
            if (boundary == nameof(IMethodBoundaryAspect.OnSuccess))
            {
                return pipeline.FlowBehaviour == FlowBehaviour.Default ||
                       pipeline.FlowBehaviour == FlowBehaviour.Return;
            }

            throw new IvorySharpException(
                $"Для обработчика '{boundary}' невозможно определить признак продолжения пайплана");
        }

        /// <summary>
        /// Возвращает признак того, что в текущем состоянии пайплайна можно выполнить перехват исходного метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        /// <returns>Признак того, что в текущем состоянии пайплайна можно выполнить перехват исходного метода.</returns>
        internal static bool CanIntercept(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehaviour != FlowBehaviour.Return;
        }
        
        /// <summary>
        /// Возвращает признак того, что в текущем состоянии пайплайна необходимо выбросить исключение.
        /// </summary>
        /// <param name="pipeline">Пайплайн выполнения.</param>
        /// <returns>Признак того, что в текущем состоянии пайплайна можно выбросить исключение.</returns>
        internal static bool ShouldThrowException(IInvocationPipeline pipeline)
        {
            return pipeline.CurrentException != null  &&
                   (pipeline.FlowBehaviour == FlowBehaviour.ThrowException || 
                    pipeline.FlowBehaviour == FlowBehaviour.RethrowException);
        }
    }
}