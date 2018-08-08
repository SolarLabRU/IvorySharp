namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal abstract class InvocationPipelineVisitor
    {
        public abstract bool CanVisit(IInvocationPipeline pipeline);
        public abstract VisitResult Visit(MethodBoundaryAspect aspect, IInvocationPipeline pipeline);
    }
}