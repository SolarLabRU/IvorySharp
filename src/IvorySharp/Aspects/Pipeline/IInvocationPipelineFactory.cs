using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Фабрика компонентов пайплайна.
    /// </summary>
    public interface IInvocationPipelineFactory
    {
        /// <summary>
        /// Создает модель пайплайна вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <param name="boundaryAspects">Аспекты типа <see cref="MethodBoundaryAspect"/>.</param>
        /// <param name="interceptionAspect">Аспект типа <see cref="MethodInterceptionAspect"/>.</param>
        /// <returns>Модель пайплайна.</returns>
        IInvocationPipeline CreatePipeline(
            IInvocation invocation,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect);

        /// <summary>
        /// Создает компонент выполнения пайплайна.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Компонент выполнения пайплайна.</returns>
        IInvocationPipelineExecutor CreateExecutor(IInvocation invocation);
    }
}