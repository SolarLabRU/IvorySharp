using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Pipeline.Visitors;
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
            var visitorAcceptor = new InvocationPipelineVisitorAcceptor(pipeline);       
            var visitResult = new VisitResult();
            
            try
            {
                visitResult = visitorAcceptor.Accept(
                    pipeline.BoundaryAspects, OnEntryVisitor.Instance);

                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Default)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                }

                // Если решили вернуть результат в OnEntry, то необходимо выполнить OnSuccess
                // так же у аспекта, решившего вернуть результат.
                var includeBreaker = visitResult.IsExecutionBreaked && 
                                     pipeline.FlowBehavior == FlowBehavior.Return;

                visitorAcceptor.AcceptBefore(
                    pipeline.BoundaryAspects, OnSuccessVisitor.Instance, 
                    visitResult.ExecutionBreaker, includeBreaker);
            }
            catch (Exception e)
            {
                // Если это исключение, сгенерированное каким-то из обработчиков -
                // прокидываем его без изменений
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException)
                    throw;

                // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
                pipeline.CurrentException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);

                // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
                pipeline.FlowBehavior = FlowBehavior.RethrowException;

                visitResult = visitorAcceptor.AcceptBefore(
                    pipeline.BoundaryAspects,
                    OnExceptionVisitor.Instance, 
                    visitResult.ExecutionBreaker, inclusive: true);
                
                var isPipelineFaulted = InvocationPipelineFlow.IsFaulted(pipeline);
                var breaker = visitResult.ExecutionBreaker;

                // Если никто не смог обработать исключение или в процессе обработки
                // появилось новое исключение - выбрасываем его наружу.
                if (isPipelineFaulted)
                    pipeline.CurrentException.Throw();

                // Если один из обработчиков решил вернуть результат вместо исключения
                // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
                if (breaker != null && !isPipelineFaulted)
                {
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    visitorAcceptor.Accept(onSuccessAspects, OnSuccessVisitor.Instance);
   
                    if (InvocationPipelineFlow.IsFaulted(pipeline))
                        pipeline.CurrentException.Throw();
                }
            }
            finally
            {
                // Если внутри аспекта произошло исключение - пайплайн поломан 
                // мы не должны вызывать OnExit - бросаем исключение во вне
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException)
                    pipeline.CurrentException.Throw();
                
                // Если решили вернуть результат, то необходимо выполнить OnExit
                // так же у аспекта, решившего вернуть результат.
                var includeBreaker = visitResult.ExecutionBreaker != null && 
                                     pipeline.FlowBehavior == FlowBehavior.Return;
                        
                visitorAcceptor.AcceptBefore(
                    pipeline.BoundaryAspects, OnExitVisitor.Instance, 
                    visitResult.ExecutionBreaker, includeBreaker);
                
                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!pipeline.Invocation.Context.Method.IsVoidReturn())
                    pipeline.Context.ReturnValue = pipeline.Invocation.Context.ReturnValue;
            }
        }
    }
}