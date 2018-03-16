using System;
using System.Collections.Generic;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент для внедрения аспектов в пайплайн выполнения метода.
    /// </summary>
    internal class MethodAspectsInjector
    {
        /// <summary>
        /// Инициализированный экземпляр компонента.
        /// </summary>
        public static MethodAspectsInjector Instance { get; } = new MethodAspectsInjector();

        private MethodAspectsInjector() { }
        
        /// <summary>
        /// Выполняет внедрение аспектов в выполнение метода.
        /// </summary>
        /// <param name="invocation">Модель выполнения метода.</param>
        /// <param name="boundaryAspects">Коллекция аспектов.</param>
        /// <param name="interceptionAspect">Аспект перехвата выполнения метода.</param>
        public void InjectAspects(IInvocation invocation, IReadOnlyCollection<IMethodBoundaryAspect> boundaryAspects, IMethodInterceptionAspect interceptionAspect)
        {
            var aggregateAspect = new AggregatedMethodBoundaryAspect(boundaryAspects);
            var pipeline = new InvocationPipeline(invocation);

            try
            {
                // Всегда выполняем OnEntry 
                aggregateAspect.OnEntry(pipeline);

                ThrowIfRequested(pipeline);
                
                if (InvocationPipelineFlow.CanIntercept(pipeline))
                {
                    // Выполнение вызова в случае, если какой-то из
                    // обработчиков не решил преждевременно покинуть выполнение 
                    // и вернуть результат
                    interceptionAspect.OnInvoke(invocation);
                }

                aggregateAspect.OnSuccess(pipeline);
                
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

                aggregateAspect.OnException(pipeline);

                // Если никто не смог обработать, то прокидываем его наверх
                if (InvocationPipelineFlow.ShouldThrowException(pipeline))
                {
                    pipeline.CurrentException.Rethrow();
                }             
                
                // Если задано несколько аспектов, то у всех до текущего
                // в случае исключения должен быть вызван OnSuccess, так как хендлер
                // решил вернуть результат вместо ошибки.
                else 
                {
                    aggregateAspect.IterateAspectsBeforeLastApplied(
                        pipeline, 
                        nameof(IMethodBoundaryAspect.OnSuccess), 
                        (a, p) => a.OnSuccess(p));
                    
                    ThrowIfRequested(pipeline);
                }
            }
            finally
            {
                aggregateAspect.OnExit(pipeline);
                ThrowIfRequested(pipeline);
                
                // В самом конце устанавливаем значение, если оно поддерживается исходным методом
                if (!invocation.Context.Method.IsVoidReturn())
                    pipeline.Context.ReturnValue = invocation.Context.ReturnValue;
            }
        }
        
        private void ThrowIfRequested(InvocationPipeline pipeline)
        {
            // Если аспект запросил выкидывание исключения
            if (InvocationPipelineFlow.ShouldThrowException(pipeline))
            {
                // Явно устанавливаем состояние, при котором обработчики исключений
                // не будут пытаться обработать данное исключение
                pipeline.FlowBehaviour = FlowBehaviour.ThrowException;
                pipeline.CurrentException.Rethrow();
            }
        }
    }
}