using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Iterators
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
        protected override bool CanContinue(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.Default ||
                   flowBehaviour == FlowBehaviour.Return;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.ThrowException ||
                   flowBehaviour == FlowBehaviour.RethrowException;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnSuccess(pipeline);
        }
    }
}