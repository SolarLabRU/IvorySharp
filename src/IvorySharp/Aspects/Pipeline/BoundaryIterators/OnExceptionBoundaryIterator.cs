using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Pipeline.BoundaryIterators;

namespace IvorySharp.Aspects.BoundaryIterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="MethodBoundaryAspect.OnException(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnExceptionBoundaryIterator : MethodBoundaryIterator
    {
        public OnExceptionBoundaryIterator(InvocationPipeline pipeline)
            : base(pipeline)
        {
        }

        /// <inheritdoc />
        protected override bool CanContinue(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.ThrowException ||
                   flowBehavior == FlowBehavior.RethrowException;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehavior flowBehavior)
        {
            return flowBehavior == FlowBehavior.Return ||
                   flowBehavior == FlowBehavior.ThrowException;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnException(pipeline);
        }
    }
}