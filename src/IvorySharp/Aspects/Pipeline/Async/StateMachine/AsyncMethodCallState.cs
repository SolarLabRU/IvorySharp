using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline.StateMachine;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.Async.StateMachine
{
    /// <summary>
    /// Состояние вызова асинхронного метода.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class AsyncMethodCallState<TPipeline> : AsyncInvocationState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        private readonly IEnumerable<MethodBoundaryAspect> _boundaryAspects;
        private readonly MethodInterceptionAspect _interceptionAspect;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncMethodCallState{TPipeline}"/>.
        /// </summary>
        public AsyncMethodCallState(
            IEnumerable<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
        {
            _boundaryAspects = boundaryAspects;
            _interceptionAspect = interceptionAspect;
        }

        /// <inheritdoc />
        public override async Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync<TResult>(TPipeline pipeline)
        {
            try
            {
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    _interceptionAspect.OnInvoke(pipeline.Invocation);
                    pipeline.CurrentReturnValue = await ((Task<TResult>) pipeline.Invocation.ReturnValue)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                // Нужно пересобрать возвращаемый результат, т.к. метод
                // мы не выполнили, а клиент ждет что-то, на чем можно вызвать await
                else
                {
                    pipeline.Invocation.ReturnValue = pipeline.IsExceptional()
                        ? Task.FromException<TResult>(pipeline.CurrentException)
                        : Task.FromResult((TResult) pipeline.CurrentReturnValue);
                }
                
                return new AsyncInvocationStateSyncAdapter<TPipeline>(
                    new SuccessState<TPipeline>(_boundaryAspects));
            }
            catch (Exception e)
            {
                var innerException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);
                
                pipeline.RethrowException(innerException);
                
                return new AsyncInvocationStateSyncAdapter<TPipeline>(
                    new CatchState<TPipeline>(_boundaryAspects));
            }
        }

        /// <inheritdoc />
        public override async Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync(TPipeline pipeline)
        {
            try
            {
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    _interceptionAspect.OnInvoke(pipeline.Invocation);
                    await ((Task) pipeline.Invocation.ReturnValue)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                else
                {
                    pipeline.Invocation.ReturnValue = pipeline.IsExceptional()
                        ? Task.FromException(pipeline.CurrentException)
                        : Task.CompletedTask;
                }

                return new AsyncInvocationStateSyncAdapter<TPipeline>(
                    new SuccessState<TPipeline>(_boundaryAspects));
            }
            catch (Exception e)
            {            
                var innerException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);
                
                pipeline.RethrowException(innerException);

                return new AsyncInvocationStateSyncAdapter<TPipeline>(
                    new CatchState<TPipeline>(_boundaryAspects));
            }
        }
    }
}