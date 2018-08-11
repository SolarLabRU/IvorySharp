using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Pipeline.Appliers;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.Synchronous
{
    /// <summary>
    /// Выполняет пайплайн <see cref="SyncAspectInvocationPipeline"/>.
    /// </summary>
    internal class SyncAspectInvocationPipelineExecutor : IInvocationPipelineExecutor
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="SyncAspectInvocationPipeline"/>.
        /// </summary>
        public static readonly SyncAspectInvocationPipelineExecutor Instance 
            = new SyncAspectInvocationPipelineExecutor();

        private SyncAspectInvocationPipelineExecutor()
        {
        }

        /// <inheritdoc />
        public void ExecutePipeline(IInvocationPipeline basePipeline)
        {
            // Это нарушает solid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (SyncAspectInvocationPipeline) basePipeline;
            var apsectReducer = new MethodAspectReducer(pipeline);
            var applyResult = new AspectApplyResult();

            try
            {
                applyResult = apsectReducer.Reduce(pipeline.BoundaryAspects, OnEntryApplier.Instance);

                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                }

                // Если решили вернуть результат в OnEntry, то необходимо выполнить OnSuccess
                // так же у аспекта, решившего вернуть результат.
                var includeBreaker = applyResult.IsExecutionBreaked && pipeline.IsReturn();

                apsectReducer.ReduceBefore(pipeline.BoundaryAspects, OnSuccessApplier.Instance,
                    applyResult.ExecutionBreaker, includeBreaker);
            }
            catch (Exception e)
            {
                // Если это исключение, сгенерированное каким-то из обработчиков
                // прокидываем его дальше 
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                    pipeline.FlowBehavior == FlowBehavior.Faulted)
                    throw;

                // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
                pipeline.CurrentException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);

                // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
                pipeline.FlowBehavior = FlowBehavior.RethrowException;

                applyResult = apsectReducer.ReduceBefore(
                    pipeline.BoundaryAspects,
                    OnExceptionApplier.Instance,
                    applyResult.ExecutionBreaker, inclusive: true);

                var breaker = applyResult.ExecutionBreaker;

                // Если один из обработчиков решил вернуть результат вместо исключения
                // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
                if (breaker != null && pipeline.IsReturn())
                {
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    apsectReducer.Reduce(onSuccessAspects, OnSuccessApplier.Instance);
                }

                // Если никто не смог обработать исключение или в процессе обработки
                // появилось новое исключение - выбрасываем его наружу.
                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
            }
            finally
            {
                // Ничего не должно выполняться, если пайплайн в сломанном состоянии.
                if (pipeline.IsFaulted())
                    pipeline.CurrentException.Throw();

                apsectReducer.ReduceBefore(
                    pipeline.BoundaryAspects, OnExitApplier.Instance,
                    applyResult.ExecutionBreaker, inclusive: true);

                // Выкидываем исключение, если пайплайн в ошибочном состоянии
                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
            }
        }
    }
}