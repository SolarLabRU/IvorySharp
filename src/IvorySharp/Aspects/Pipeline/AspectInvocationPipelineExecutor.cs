using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Pipeline.Appliers;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Выполняет пайплайн <see cref="AspectInvocationPipeline"/>.
    /// </summary>
    internal class AspectInvocationPipelineExecutor : IPipelineExecutor
    {
        /// <summary>
        /// Инициаилизированный экземпляр <see cref="AspectInvocationPipeline"/>.
        /// </summary>
        public static readonly AspectInvocationPipelineExecutor Instance = new AspectInvocationPipelineExecutor();

        private AspectInvocationPipelineExecutor() { }
        
        /// <inheritdoc />
        public void ExecutePipeline(IInvocationPipeline basePipeline)
        {
            // Это нарушает soLid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (AspectInvocationPipeline) basePipeline;
            var apsectReducer = new MethodAspectReducer(pipeline);       
            var visitResult = new AspectApplyResult();
            
            try
            {
                visitResult = apsectReducer.Reduce(
                    pipeline.BoundaryAspects, OnEntryApplier.Instance);

                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                }

                // Если решили вернуть результат в OnEntry, то необходимо выполнить OnSuccess
                // так же у аспекта, решившего вернуть результат.
                var includeBreaker = visitResult.IsExecutionBreaked && 
                                     pipeline.FlowBehavior == FlowBehavior.Return;

                apsectReducer.ReduceBefore(
                    pipeline.BoundaryAspects, OnSuccessApplier.Instance, 
                    visitResult.ExecutionBreaker, includeBreaker);
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

                visitResult = apsectReducer.ReduceBefore(
                    pipeline.BoundaryAspects,
                    OnExceptionApplier.Instance, 
                    visitResult.ExecutionBreaker, inclusive: true);
                
                var breaker = visitResult.ExecutionBreaker;
        
                // Если один из обработчиков решил вернуть результат вместо исключения
                // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
                if (breaker != null && !pipeline.IsExceptional)
                {
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    apsectReducer.Reduce(onSuccessAspects, OnSuccessApplier.Instance);
                }
                
                // Если никто не смог обработать исключение или в процессе обработки
                // появилось новое исключение - выбрасываем его наружу.
                if (pipeline.IsExceptional)
                    pipeline.CurrentException.Throw();
            }
            finally
            {      
                // Ничего не должно выполняться, если пайплайн в сломанном состоянии.
                if (pipeline.IsFaulted)
                    pipeline.CurrentException.Throw();
             
                apsectReducer.ReduceBefore(
                    pipeline.BoundaryAspects, OnExitApplier.Instance, 
                    visitResult.ExecutionBreaker, inclusive: true);
                
                // Выкидываем исключение, если пайплайн в ошибочном состоянии
                if (pipeline.IsExceptional)
                    pipeline.CurrentException.Throw();
                
                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!pipeline.Invocation.Context.Method.IsVoidReturn())
                    pipeline.Context.ReturnValue = pipeline.Invocation.Context.ReturnValue;
            }
        }
    }
}