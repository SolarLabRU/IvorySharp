namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal class OnSuccessVisitor : InvocationPipelineVisitor
    {
        public static readonly OnSuccessVisitor Instance = new OnSuccessVisitor();
        
        private OnSuccessVisitor() { }
        
        public override bool CanVisit(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.Default ||
                   pipeline.FlowBehavior == FlowBehavior.Return;
        }

        public override VisitResult Visit(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnSuccess(pipeline);
            
            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                              pipeline.FlowBehavior == FlowBehavior.RethrowException;
            
            return new VisitResult(shouldBreak ? aspect : null);
        }
    }
}