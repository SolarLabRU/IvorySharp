using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.BoundaryIterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="MethodBoundaryAspect.OnEntry(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnEntryBoundaryIterator : MethodBoundaryIterator
    {
        public OnEntryBoundaryIterator(InvocationPipeline pipeline)
            : base(pipeline)
        {
        }

        /// <inheritdoc />
        protected override bool CanContinue(FlowBehavior flowBehavior)
        {
            return flowBehavior != FlowBehavior.ThrowException &&
                   flowBehavior != FlowBehavior.RethrowException &&
                   flowBehavior != FlowBehavior.Return;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.RethrowException ||
                   flowBehavior == FlowBehavior.ThrowException ||
                   flowBehavior == FlowBehavior.Return;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnEntry(pipeline);
        }
    }
}