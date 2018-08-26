using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние завершения вызова.
    /// Вызывается независимо от того, с каким исходом выполнился перехватываемый метод.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class FinallyState<TPipeline> : MethodBoundaryState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="FinallyState{TPipeline}"/>.
        /// </summary>
        public FinallyState(IEnumerable<MethodBoundaryAspect> boundaryAspects) 
            : base(boundaryAspects)
        {
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnExit(pipeline);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool ShouldBreak(TPipeline pipeline, MethodBoundaryAspect aspect,  out IInvocationState<TPipeline> transition)
        {
            var flow = pipeline.FlowBehavior;

            switch (flow)
            {
                case FlowBehavior.Return:
                case FlowBehavior.ThrowException:
                    transition = new FinallyState<TPipeline>(BoundaryAspects.TakeBeforeExclusive(aspect));
                    break;

                default:
                    transition = null; break;
            }

            return transition != null;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override IInvocationState<TPipeline> CreateContinuation()
        {
            return CompleteState<TPipeline>.Instance;
        }
    }
}