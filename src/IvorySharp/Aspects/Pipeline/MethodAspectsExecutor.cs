using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Iterators;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент, выполняющий прикрепленные аспекты во время вызова метода.
    /// </summary>
    internal class MethodAspectsExecutor
    {
        /// <summary>
        /// Инициализированный экземпляр компонента.
        /// </summary>
        public static MethodAspectsExecutor Instance { get; } = new MethodAspectsExecutor();

        private MethodAspectsExecutor() { }
        
        /// <summary>
        /// Выполняет аспекты и вызов метода.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        /// <param name="boundaryAspects">Аспекты, которые должны выполняться в точках прикрепления.</param>
        /// <param name="interceptionAspect">Аспект, который должен перехватывать вызов основного метода.</param>
        public void ExecuteAspects(
            IInvocation invocation, 
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
        {
            var pipeline = new InvocationPipeline(invocation);
            
            var onEntryIterator = new OnEntryMethodBoundaryIterator(pipeline);
            var onExitIterator = new OnExitMethodBoundaryIterator(pipeline);
            var onSuccessIterator = new OnSuccessMethodBoundaryIterator(pipeline);
            var stateAwareIteratorWrapper = new StateAwareIteratorWrapper();

            try
            {
                stateAwareIteratorWrapper.Iterate(onEntryIterator, boundaryAspects);
                
                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehaviour == FlowBehaviour.Default)
                {
                    interceptionAspect.OnInvoke(invocation);
                }

                stateAwareIteratorWrapper.Iterate(onSuccessIterator, boundaryAspects); 
            }
            catch (Exception e)
            {
                // Если это исключение, сгенерированное каким-то из обработчиков -
                // прокидываем его без изменений
                if (pipeline.FlowBehaviour == FlowBehaviour.ThrowException)
                    throw;

                // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
                pipeline.CurrentException = e.UnwrapIf(e is TargetInvocationException && e.InnerException != null);

                // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
                pipeline.FlowBehaviour = FlowBehaviour.RethrowException;

                var onExceptionResult = stateAwareIteratorWrapper.Iterate(
                    new OnExceptionMethodBoundaryIterator(pipeline), boundaryAspects, throwIfPipelineFaulted: false);
                
                var isPipelineFaulted = InvocationPipelineFlow.IsFaulted(pipeline);
                var breaker = onExceptionResult.Breaker;
                
                // Если никто не смог обработать исключение или в процессе обработки
                // появилось новое исключение - выбрасываем его наружу.
                if (isPipelineFaulted)
                    pipeline.CurrentException.Rethrow();
                
                // Если один из обработчиков решил вернуть результат вместо исключения
                // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
                if (breaker != null && !isPipelineFaulted)
                {
                    var onSuccessAspects = boundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    onSuccessIterator.Iterate(onSuccessAspects);
                    
                    if (InvocationPipelineFlow.IsFaulted(pipeline))
                        pipeline.CurrentException.Rethrow();
                }
            }
            finally
            {
                stateAwareIteratorWrapper.Iterate(onExitIterator, boundaryAspects);
                
                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!invocation.Context.Method.IsVoidReturn())
                    pipeline.Context.ReturnValue = invocation.Context.ReturnValue;
            }
        }
        
        /// <summary>
        /// Класс-обертка для вызова итератора аспектов.
        /// </summary>
        private class StateAwareIteratorWrapper
        {
            private MethodBoundaryAspect _currentBreaker;

            public MethodBoundaryIterator.BoundaryIterationResult Iterate(
                MethodBoundaryIterator iterator,
                IReadOnlyCollection<MethodBoundaryAspect> aspects,
                bool throwIfPipelineFaulted = true)
            {
                var result = _currentBreaker == null
                    ? iterator.Iterate(aspects)
                    : iterator.Iterate(aspects, _currentBreaker.Order);

                if (throwIfPipelineFaulted)
                {
                    var isExceptionalState = InvocationPipelineFlow.IsFaulted(iterator.Pipeline);
                    if (isExceptionalState)
                        iterator.Pipeline.CurrentException.Rethrow();
                }

                _currentBreaker = result.Breaker;
            
                return result;
            }
        }
    }
}