using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент, выполняющий пайплан вызова метода.
    /// </summary>
    [PublicAPI]
    public interface IInvocationPipelineExecutor
    {
        /// <summary>
        /// Выполняет пайплайн вызова метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова метода.</param>
        /// <param name="invocation">Вызов метода.</param>
        void ExecutePipeline([NotNull] IInvocationPipeline pipeline, [NotNull] IInvocation invocation);
    }
}