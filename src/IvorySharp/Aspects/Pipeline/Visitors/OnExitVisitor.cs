namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal class OnExitVisitor : InvocationPipelineVisitor
    {
        public static readonly OnExitVisitor Instance = new OnExitVisitor();
        
        private OnExitVisitor() { }

        
        public override bool CanVisit(IInvocationPipeline pipeline)
        {
            return true;
        }

        public override VisitResult Visit(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnExit(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.RethrowException ||
                              pipeline.FlowBehavior == FlowBehavior.ThrowException;

            return new VisitResult(shouldBreak ? aspect : null);
        }
    }
}