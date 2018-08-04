using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.BoundaryIterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="MethodBoundaryAspect.OnExit(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnExitMethodBoundaryIterator : MethodBoundaryIterator
    {
        public OnExitMethodBoundaryIterator(InvocationPipeline pipeline)
            : base(pipeline)
        {
        }

        /// <inheritdoc />
        protected override bool CanContinue(FlowBehavior flowBehavior)
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.RethrowException ||
                   flowBehavior == FlowBehavior.Return ||
                   flowBehavior == FlowBehavior.ThrowException;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnExit(pipeline);
        }
    }
}