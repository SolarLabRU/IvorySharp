using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Iterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="MethodBoundaryAspect.OnEntry(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnEntryMethodBoundaryIterator : MethodBoundaryIterator
    {
        public OnEntryMethodBoundaryIterator(InvocationPipeline pipeline) 
            : base(pipeline)
        {
        }

        /// <inheritdoc />
        protected override bool CanContinue(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour != FlowBehaviour.ThrowException &&
                   flowBehaviour != FlowBehaviour.RethrowException &&
                   flowBehaviour != FlowBehaviour.Return;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.RethrowException ||
                   flowBehaviour == FlowBehaviour.ThrowException ||
                   flowBehaviour == FlowBehaviour.Return;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnEntry(pipeline);
        }
    }
}