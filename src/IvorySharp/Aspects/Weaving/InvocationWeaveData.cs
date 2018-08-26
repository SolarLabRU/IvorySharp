using System.Collections.Generic;
using System.ComponentModel;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Данные о вызове.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InvocationWeaveData
    {
        /// <summary>
        /// Признак того, что на метод возможно применить обвязку из аспектов.
        /// </summary>
        public bool IsWeaveable { get; private set; }

        /// <summary>
        /// Пайплайн вызова.
        /// </summary>
        public IInvocationPipeline Pipeline { get; private set; }

        /// <summary>
        /// Компонент для выполнения пайплайна.
        /// </summary>
        public IInvocationPipelineExecutor PipelineExecutor { get; private set; }

        /// <summary>
        /// Аспекты типа <see cref="MethodBoundaryAspect"/>.
        /// </summary>
        public IReadOnlyCollection<MethodBoundaryAspect> BoundaryAspects { get; private set; }

        /// <summary>
        /// Аспект типа <see cref="MethodInterceptionAspect"/>.
        /// </summary>
        public MethodInterceptionAspect InterceptionAspect { get; private set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationWeaveData"/>.
        /// </summary>
        private InvocationWeaveData()
        {
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationWeaveData"/> с признаком
        /// невозможности применения аспектов.
        /// </summary>
        /// <returns>Экземляр <see cref="InvocationWeaveData"/>.</returns>
        internal static InvocationWeaveData Unweavable()
        {
            return new InvocationWeaveData
            {
                IsWeaveable = false,
            };
        }

        /// <summary>
        /// Создает экземпляр <see cref="InvocationWeaveData"/> с признаком
        /// возможности применения аспектов.
        /// </summary>
        /// <param name="pipeline">Пайплайн.</param>
        /// <param name="pipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="boundaryAspects">Аспекты <see cref="MethodBoundaryAspect"/>.</param>
        /// <param name="interceptionAspect">Аспект <see cref="MethodInterceptionAspect"/>.</param>
        /// <returns>Экземляр <see cref="InvocationWeaveData"/>.</returns>
        internal static InvocationWeaveData Weavable(
            IInvocationPipeline pipeline,
            IInvocationPipelineExecutor pipelineExecutor,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect)
        {
            return new InvocationWeaveData
            {
                IsWeaveable = true,
                Pipeline = pipeline,
                PipelineExecutor = pipelineExecutor,
                BoundaryAspects = boundaryAspects,
                InterceptionAspect = interceptionAspect
            };
        }
    }
}