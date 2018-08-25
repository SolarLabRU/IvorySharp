using System.ComponentModel;
using IvorySharp.Core;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Фабрика компонентов пайплайна.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInvocationPipelineFactory : IComponent
    {
        /// <summary>
        /// Создает модель пайплайна вызова метода.
        /// </summary>
        /// <param name="signature">Модель вызова метода.</param>
        /// <param name="boundaryAspects">Аспекты типа <see cref="MethodBoundaryAspect"/>.</param>
        /// <param name="interceptionAspect">Аспект типа <see cref="MethodInterceptionAspect"/>.</param>
        /// <returns>Модель пайплайна.</returns>
        IInvocationPipeline CreatePipeline(IInvocationSignature signature,
            MethodBoundaryAspect[] boundaryAspects, 
            MethodInterceptionAspect interceptionAspect);

        /// <summary>
        /// Создает компонент выполнения пайплайна вызова метода.
        /// </summary>
        /// <param name="signature">Контекст вызова.</param>
        /// <returns>Компонент выполнения пайплайна.</returns>
        IInvocationPipelineExecutor CreateExecutor(IInvocationSignature signature);
    }
}