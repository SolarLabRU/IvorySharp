using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Pipeline.StateMachine;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Выполняет пайплайн <see cref="InvocationPipeline"/>.
    /// </summary>
    internal sealed class InvocationPipelineExecutor : IInvocationPipelineExecutor
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="InvocationPipeline"/>.
        /// </summary>
        public static readonly InvocationPipelineExecutor Instance 
            = new InvocationPipelineExecutor();

        private InvocationPipelineExecutor() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePipeline(IInvocationPipeline basePipeline, IInvocation invocation)
        {
            // Это нарушает solid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (InvocationPipeline) basePipeline;
            pipeline.Init(invocation);

            var entryState = new EntryState<InvocationPipeline>(
                pipeline.BoundaryAspects, pipeline.InterceptionAspect);
            
            InvocationStateMachine.Execute(pipeline, entryState);
        }
    }
}