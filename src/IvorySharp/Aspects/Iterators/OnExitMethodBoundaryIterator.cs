using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Iterators
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
        protected override bool CanContinue(FlowBehaviour flowBehaviour)
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.RethrowException ||
                   flowBehaviour == FlowBehaviour.Return ||
                   flowBehaviour == FlowBehaviour.ThrowException;
        }  

        /// <inheritdoc />
        protected override void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnExit(pipeline);
        }
    }
}