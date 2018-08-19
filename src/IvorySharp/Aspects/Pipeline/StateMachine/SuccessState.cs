using System.Collections.Generic;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние успешного выполнения метода (без исключения)
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class SuccessState<TPipeline> : MethodBoundaryState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="SuccessState{TPipeline}"/>.
        /// </summary>
        public SuccessState(IEnumerable<MethodBoundaryAspect> boundaryAspects) 
            : base(boundaryAspects)
        {
        }

        /// <inheritdoc />
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnSuccess(pipeline);
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

                case FlowBehavior.RethrowException:
                    transition = new CatchState<TPipeline>(BoundaryAspects.TakeBefore(data.CurrentAspect));
                    break;
                
                default:
                    transition = null; break;
            }
            
            return transition != null;
        }

        /// <inheritdoc />
        protected override InvocationState<TPipeline> CreateContinuation()
        {
            return new FinallyState<TPipeline>(BoundaryAspects);
        }
    }
}