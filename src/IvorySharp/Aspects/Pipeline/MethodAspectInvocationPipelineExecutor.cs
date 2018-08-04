using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.BoundaryIterators;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Выполняет пайплайн <see cref="MethodAspectInvocationPipeline"/>.
    /// </summary>
    internal class MethodAspectInvocationPipelineExecutor : IMethodAspectPipelineExecutor
    {
        /// <summary>
        /// Инициаилизированный экземпляр <see cref="MethodAspectInvocationPipeline"/>.
        /// </summary>
        public static readonly MethodAspectInvocationPipelineExecutor Instance = new MethodAspectInvocationPipelineExecutor();

        private MethodAspectInvocationPipelineExecutor() { }

        /// <inheritdoc />
        public void ExecutePipeline(IInvocationPipeline basePipeline)
        {
            // Это нарушает soLid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (MethodAspectInvocationPipeline) basePipeline;

            var onEntryIterator = new OnEntryMethodBoundaryIterator(pipeline);
            var onExitIterator = new OnExitMethodBoundaryIterator(pipeline);
            var onSuccessIterator = new OnSuccessMethodBoundaryIterator(pipeline);
            var stateAwareMetaIterator = new PipelineStateAwareMetaIterator();

            try
            {
                stateAwareMetaIterator.Iterate(onEntryIterator, pipeline.BoundaryAspects);

                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Default)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                }

                stateAwareMetaIterator.Iterate(onSuccessIterator, pipeline.BoundaryAspects);
            }
            catch (Exception e)
            {
                // Если это исключение, сгенерированное каким-то из обработчиков -
                // прокидываем его без изменений
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException)
                    throw;

                // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
                pipeline.CurrentException = e.UnwrapIf(e is TargetInvocationException && e.InnerException != null);

                // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
                pipeline.FlowBehavior = FlowBehavior.RethrowException;

                var onExceptionResult = stateAwareMetaIterator.Iterate(
                    new OnExceptionMethodBoundaryIterator(pipeline),
                    pipeline.BoundaryAspects, throwIfPipelineFaulted: false);

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
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();
                    
                    onSuccessIterator.Iterate(onSuccessAspects);

                    if (InvocationPipelineFlow.IsFaulted(pipeline))
                        pipeline.CurrentException.Rethrow();
                }
            }
            finally
            {
                stateAwareMetaIterator.Iterate(onExitIterator, pipeline.BoundaryAspects);

                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!pipeline.Invocation.Context.Method.IsVoidReturn())
                    pipeline.Context.ReturnValue = pipeline.Invocation.Context.ReturnValue;
            }
        }

        /// <summary>
        /// Класс-обертка для вызова итератора аспектов.
        /// </summary>
        private class PipelineStateAwareMetaIterator
        {
            private MethodBoundaryAspect _currentBreaker;

            /// <summary>
            /// Выполняет обход аспектов, запоминая состояние того, кто прервал цепочку вызовов.
            /// </summary>
            /// <param name="iterator">Итератор аспектов.</param>
            /// <param name="aspects">Коллекция аспектов.</param>
            /// <param name="throwIfPipelineFaulted">Признак необходимости выбросить исключение, если при выполнении пайплайна случилась или была сгенерирована ошибка.</param>
            /// <returns>Результат обхода аспектов.</returns>
            public MethodBoundaryIterator.BoundaryIterationResult Iterate(
                MethodBoundaryIterator iterator,
                IReadOnlyCollection<MethodBoundaryAspect> aspects,
                bool throwIfPipelineFaulted = true)
            {
                var result = _currentBreaker == null
                    ? iterator.Iterate(aspects)
                    : iterator.Iterate(aspects, _currentBreaker.InternalOrder);

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