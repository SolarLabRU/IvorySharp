using System.Collections.Generic;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние начала вызова метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class EntryState<TPipeline> : MethodBoundaryState<TPipeline>
        where TPipeline : InvocationPipelineBase
    {
        private readonly MethodInterceptionAspect _interceptionAspect;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="EntryState{TPipeline}"/>.
        /// </summary>
        public EntryState(
            IEnumerable<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect) 
            : base(boundaryAspects)
        {
            _interceptionAspect = interceptionAspect;
        }

        /// <inheritdoc />
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnEntry(pipeline);
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(BoundaryStateData data, out InvocationState<TPipeline> transition)
        {
            var flow = data.Pipeline.FlowBehavior;
            switch (flow)
            {
                // OnEntry -> Return(value) -> OnSuccess [only executed] -> OnExit [only executed] 
                case FlowBehavior.Return:
                    transition = new SuccessState<TPipeline>(BoundaryAspects.TakeBefore(data.CurrentAspect));
                    break;
                
                // OnEntry -> Throw(exception) -> OnExit [only executed]
                case FlowBehavior.ThrowException:
                    transition = new FinallyState<TPipeline>(BoundaryAspects.TakeBefore(data.CurrentAspect));
                    break;
                
                // OnEntry -> Rethrow(exception) -> OnException [only executed] -> {maybe: OnSuccess} -> OnExit [only executed]
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
            return new MethodCallState<TPipeline>(_interceptionAspect, BoundaryAspects);
        }
    }
}