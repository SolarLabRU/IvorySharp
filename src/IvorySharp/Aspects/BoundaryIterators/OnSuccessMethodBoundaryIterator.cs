using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.BoundaryIterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="MethodBoundaryAspect.OnSuccess(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnSuccessMethodBoundaryIterator : MethodBoundaryIterator
    {
        public OnSuccessMethodBoundaryIterator(InvocationPipeline pipeline)
            : base(pipeline)
        {
        }

        /// <inheritdoc />
        protected override bool CanContinue(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.Default ||
                   flowBehavior == FlowBehavior.Return;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.ThrowException ||
                   flowBehavior == FlowBehavior.RethrowException;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnSuccess(pipeline);
        }
    }
}