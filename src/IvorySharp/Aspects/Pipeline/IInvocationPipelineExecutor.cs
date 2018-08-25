using System.ComponentModel;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент, выполняющий пайплан вызова метода.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInvocationPipelineExecutor
    {
        /// <summary>
        /// Выполняет пайплайн вызова метода.
        /// </summary>
        /// <param name="pipeline">Пайплайн вызова метода.</param>
        /// <param name="invocation">Вызов метода.</param>
        void ExecutePipeline(IInvocationPipeline pipeline, IInvocation invocation);
    }
}