namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal struct VisitResult
    {
        public readonly MethodBoundaryAspect ExecutionBreaker;
        public readonly bool IsExecutionBreaked;

        public VisitResult(MethodBoundaryAspect executionBreaker)
        {
            ExecutionBreaker = executionBreaker;
            IsExecutionBreaked = executionBreaker != null;
        }
    }
}