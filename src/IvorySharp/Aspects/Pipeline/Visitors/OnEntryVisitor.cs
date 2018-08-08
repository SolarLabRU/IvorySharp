namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal class OnEntryVisitor : InvocationPipelineVisitor
    {
        public static readonly OnEntryVisitor Instance = new OnEntryVisitor();
        
        private OnEntryVisitor() { }
            
        public override bool CanVisit(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior != FlowBehavior.ThrowException &&
                   pipeline.FlowBehavior != FlowBehavior.RethrowException &&
                   pipeline.FlowBehavior != FlowBehavior.Return;
        }

        public override VisitResult Visit(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnEntry(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.RethrowException ||
                              pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                              pipeline.FlowBehavior == FlowBehavior.Return;

            return new VisitResult(shouldBreak ? aspect : null);
        }
    }
}