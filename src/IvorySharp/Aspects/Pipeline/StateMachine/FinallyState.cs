using System.Collections.Generic;
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
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnExit(pipeline);
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(BoundaryStateData data, out InvocationState<TPipeline> transition)
        {
            var flow = data.Pipeline.FlowBehavior;

            switch (flow)
            {
                case FlowBehavior.Return:
                case FlowBehavior.ThrowException:
                    transition = new FinallyState<TPipeline>(BoundaryAspects.TakeBefore(data.CurrentAspect));
                    break;

                default:
                    transition = null; break;
            }

            return transition != null;
        }

        /// <inheritdoc />
        protected override InvocationState<TPipeline> CreateContinuation()
        {
            return CompleteState<TPipeline>.Instance;
        }
    }
}