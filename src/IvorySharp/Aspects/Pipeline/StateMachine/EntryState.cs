using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnEntry(pipeline);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override bool ShouldBreak(TPipeline pipeline, MethodBoundaryAspect aspect, out IInvocationState<TPipeline> transition)
        {
            var flow = pipeline.FlowBehavior;
            switch (flow)
            {
                // OnEntry -> Return(value) -> OnSuccess [only executed] -> OnExit [only executed] 
                case FlowBehavior.Return:
                    transition = new SuccessState<TPipeline>(BoundaryAspects.TakeBeforeExclusive(aspect));
                    break;
                
                // OnEntry -> Throw(exception) -> OnExit [only executed]
                case FlowBehavior.ThrowException:
                    transition = new FinallyState<TPipeline>(BoundaryAspects.TakeBeforeExclusive(aspect));
                    break;
                
                // OnEntry -> Rethrow(exception) -> OnException [all except current] -> {maybe: OnSuccess} -> OnExit
                case FlowBehavior.RethrowException:
                    transition = new CatchState<TPipeline>(BoundaryAspects)
                    {
                        IgnoredAspects = new[]{ aspect }
                    };
                    break;
                
                // OnEntry -> ContinueFaulted(value) -> OnSuccess [all] -> OnExit
                case FlowBehavior.UpdateReturnValue:
                    transition = new SuccessState<TPipeline>(BoundaryAspects);
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
            return new MethodCallState<TPipeline>(_interceptionAspect, BoundaryAspects);
        }
    }
}