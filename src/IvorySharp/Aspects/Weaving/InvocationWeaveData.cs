using System.Collections.Generic;
using System.ComponentModel;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Данные о вызове.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InvocationWeaveData
    {
        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        public MethodLambda MethodInvoker { get; private set; }
        
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
        private InvocationWeaveData() { }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationWeaveData"/> с признаком
        /// невозможности применения аспектов.
        /// </summary>
        /// <param name="methodInvoker">Делегал для быстрого вызова метода.</param>
        /// <returns>Экземляр <see cref="InvocationWeaveData"/>.</returns>
        internal static InvocationWeaveData Unweavable(MethodLambda methodInvoker)
        {
            return new InvocationWeaveData
            {
                IsWeaveable = false,
                MethodInvoker = methodInvoker
            };
        }

        /// <summary>
        /// Создает экземпляр <see cref="InvocationWeaveData"/> с признаком
        /// возможности применения аспектов.
        /// </summary>
        /// <param name="methodInvoker">Делегал для быстрого вызова метода.</param>
        /// <param name="pipeline">Пайплайн.</param>
        /// <param name="pipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="boundaryAspects">Аспекты <see cref="MethodBoundaryAspect"/>.</param>
        /// <param name="interceptionAspect">Аспект <see cref="MethodInterceptionAspect"/>.</param>
        /// <returns>Экземляр <see cref="InvocationWeaveData"/>.</returns>
        internal static InvocationWeaveData Weavable(
             MethodLambda methodInvoker,
            IInvocationPipeline pipeline,
            IInvocationPipelineExecutor pipelineExecutor,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect)
        {
            return new InvocationWeaveData
            {
                IsWeaveable = true,
                MethodInvoker = methodInvoker,
                Pipeline = pipeline,
                PipelineExecutor = pipelineExecutor,
                BoundaryAspects = boundaryAspects,
                InterceptionAspect = interceptionAspect
            };
        }
    }
}