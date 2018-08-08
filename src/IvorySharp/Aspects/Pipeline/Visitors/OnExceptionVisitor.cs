namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal class OnExceptionVisitor : InvocationPipelineVisitor
    {
        public static readonly OnExceptionVisitor Instance = new OnExceptionVisitor();
        
        private OnExceptionVisitor() { }
        
        public override bool CanVisit(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                   pipeline.FlowBehavior == FlowBehavior.RethrowException;
        }

        public override VisitResult Visit(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnException(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.Return ||
                              pipeline.FlowBehavior == FlowBehavior.ThrowException;

            return new VisitResult(shouldBreak ? aspect : null);
        }
    }
}