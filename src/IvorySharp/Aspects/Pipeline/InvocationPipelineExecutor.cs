using IvorySharp.Aspects.Pipeline.StateMachine;

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
        
        public void ExecutePipeline(IInvocationPipeline basePipeline)
        {
            // Это нарушает solid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (InvocationPipeline) basePipeline;
            var stateMachine = new InvocationStateMachine<InvocationPipeline>(pipeline);  
            
            stateMachine.Execute(new EntryState<InvocationPipeline>(
                pipeline.BoundaryAspects,
                pipeline.InterceptionAspect));
        }
    }
}