using System.Collections.Generic;
using System.Linq;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Состояние неудачного выполнения метода (возникло исключение)
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class CatchState<TPipeline> : MethodBoundaryState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="CatchState{TPipeline}"/>.
        /// </summary>
        public CatchState(IEnumerable<MethodBoundaryAspect> boundaryAspects) 
            : base(boundaryAspects)
        {
        }
        
        /// <inheritdoc />
        protected override void Apply(MethodBoundaryAspect aspect, TPipeline pipeline)
        {
            aspect.OnException(pipeline);
        }

        /// <inheritdoc />
        protected override bool ShouldBreak(TPipeline pipeline, MethodBoundaryAspect aspect, out InvocationState<TPipeline> transition)
        {
            var flow = pipeline.FlowBehavior;

            switch (flow)
            {
                // OnEntry -> MethodCall [exception] -> Catch [return] -> OnSuccess [only executed]
                // для уже выполнившихся до Return аспектов должно казаться что метод выполнился без
                // ошибок и вернул результат.
                case FlowBehavior.Return:
                    transition = new SuccessState<TPipeline>(BoundaryAspects.TakeBeforeExclusive(aspect));
                    break;
                
                case FlowBehavior.ThrowException:
                    transition = new FinallyState<TPipeline>(BoundaryAspects.TakeBeforeExclusive(aspect));
                    break;

                // OnEntry -> MethodCall [exception] -> Catch [Swallow] -> OnSuccess [all except executed]
                case FlowBehavior.UpdateReturnValue:
                    transition = new SuccessState<TPipeline>(BoundaryAspects)
                    {
                        IgnoredAspects = BoundaryAspects.TakeBeforeInclusive(aspect)
                            .ToArray()
                    };
                    break;
                
                default:
                    transition = null;
                    break;
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