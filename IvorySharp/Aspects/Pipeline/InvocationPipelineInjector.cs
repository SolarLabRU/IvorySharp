using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    public interface IInvocationPipelineInjector
    {
        void Inject(IInvocation invocation);
    }
    
    /// <summary>
    /// Компонент для внедрения пайплайна выполнения метода.
    /// </summary>
    internal class InvocationPipelineInjector : IInvocationPipelineInjector
    {
        private readonly IMethodBoundaryAspect _methodBoundaryAspect;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="InvocationPipelineInjector"/>.
        /// </summary>
        /// <param name="boundaryAspects">Список аспектов.</param>
        internal InvocationPipelineInjector(IReadOnlyCollection<IMethodBoundaryAspect> boundaryAspects)
        {
            _methodBoundaryAspect = new AggregatedMethodBoundaryAspect(boundaryAspects);
        }

        /// <inheritdoc />
        public void Inject(IInvocation invocation)
        {
            var pipeline = new InvocationPipeline(invocation.Context);

            try
            {
                // Всегда выполняем OnEntry 
                _methodBoundaryAspect.OnEntry(pipeline);

                ThrowIfRequested(pipeline);
                
                if (InvocationPipelineFlow.CanIntercept(pipeline))
                {
                    // Выполнение вызова в случае, если какой-то из
                    // обработчиков не решил преждевременно покинуть выполнение 
                    // и вернуть результат
                    invocation.Proceed();
                }

                _methodBoundaryAspect.OnSuccess(pipeline);
                
                ThrowIfRequested(pipeline);
            }
            catch (Exception e)
            {
                // Если исключение сгенерировал какой-то из обработчиков, то прокидываем его в таком виде как есть
                if (ReferenceEquals(e, pipeline.CurrentException) && pipeline.FlowBehaviour == FlowBehaviour.ThrowException)
                    throw;
                
                // Распаковываем исключение, если оно связано с рефлексией
                pipeline.CurrentException = e.UnwrapIf(e is TargetInvocationException);
                
                // Устанавливаем состояние прокидывания исключений по цепочке хендлеров
                pipeline.FlowBehaviour = FlowBehaviour.RethrowException;

                _methodBoundaryAspect.OnException(pipeline);

                // Если никто не смог обработать, то прокидываем его наверх
                if (InvocationPipelineFlow.CanThrow(pipeline))
                {
                    pipeline.CurrentException.Rethrow();
                }             
                // Если задано несколько аспектов, то у всех до текущего
                // в случае исключения должен быть вызван OnSuccess, так как хендлер
                // решил вернуть результат вместо ошибки.
                else if (_methodBoundaryAspect is AggregatedMethodBoundaryAspect aggregate)
                {
                    aggregate.IterateAspectsBeforeCurrent(
                        pipeline, 
                        nameof(IMethodBoundaryAspect.OnSuccess), 
                        (a, p) => a.OnSuccess(p));
                    
                    ThrowIfRequested(pipeline);
                }
            }
            finally
            {
                _methodBoundaryAspect.OnExit(pipeline);
                ThrowIfRequested(pipeline);
                
                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!invocation.Context.Method.IsVoidReturn())
                    pipeline.InvocationContext.ReturnValue = invocation.Context.ReturnValue;
            }
        }

        private void ThrowIfRequested(InvocationPipeline pipeline)
        {
            // Если аспект запросил выкидывание исключения
            if (InvocationPipelineFlow.CanThrow(pipeline))
            {
                // Явно устанавливаем состояние, при котором обработчики исключений
                // не будут пытаться обработать данное исключение
                pipeline.FlowBehaviour = FlowBehaviour.ThrowException;
                pipeline.CurrentException.Rethrow();
            }
        }
    }
}