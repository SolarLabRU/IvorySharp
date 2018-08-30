using IvorySharp.Core;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Фабрика компонентов пайплайна.
    /// </summary>
    [PublicAPI]
    public interface IInvocationPipelineFactory : IComponent
    {
        /// <summary>
        /// Создает модель пайплайна вызова метода.
        /// </summary>
        /// <param name="signature">Модель вызова метода.</param>
        /// <param name="boundaryAspects">Аспекты типа <see cref="MethodBoundaryAspect"/>.</param>
        /// <param name="interceptionAspect">Аспект типа <see cref="MethodInterceptionAspect"/>.</param>
        /// <returns>Модель пайплайна.</returns>
        [NotNull] IInvocationPipeline CreatePipeline([NotNull] IInvocationSignature signature,
            [NotNull] MethodBoundaryAspect[] boundaryAspects, 
            [NotNull] MethodInterceptionAspect interceptionAspect);

        /// <summary>
        /// Создает компонент выполнения пайплайна вызова метода.
        /// </summary>
        /// <param name="signature">Контекст вызова.</param>
        /// <returns>Компонент выполнения пайплайна.</returns>
        [NotNull] IInvocationPipelineExecutor CreateExecutor([NotNull] IInvocationSignature signature);
    }
}