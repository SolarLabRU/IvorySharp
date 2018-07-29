using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Iterators
{
    /// <summary>
    /// Итератор для точки прикрепления <see cref="IMethodBoundaryAspect.OnException(IInvocationPipeline)"/>.
    /// </summary>
    internal class OnExceptionMethodBoundaryIterator : MethodBoundaryIterator
    {
        public OnExceptionMethodBoundaryIterator(InvocationPipeline pipeline) 
            : base(pipeline)
        {
        }
        
        /// <inheritdoc />
        protected override bool CanContinue(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.ThrowException ||
                   flowBehaviour == FlowBehaviour.RethrowException;
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(FlowBehaviour flowBehaviour)
        {
            return flowBehaviour == FlowBehaviour.Return ||
                   flowBehaviour == FlowBehaviour.ThrowException;
        }

        /// <inheritdoc />
        protected override void ExecuteAspect(IMethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnException(pipeline);
        }
    }
}