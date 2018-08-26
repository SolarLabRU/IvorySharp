using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline.StateMachine;

namespace IvorySharp.Aspects.Pipeline.Async.StateMachine
{
    /// <summary>
    /// Адаптирует состояние синхронного вызова под асинхронное.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class AsyncInvocationStateSyncAdapter<TPipeline> : AsyncInvocationState<TPipeline>
        where TPipeline : InvocationPipelineBase
    {
        private readonly IInvocationState<TPipeline> _invocationState;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationStateSyncAdapter{TPipeline}"/>.
        /// </summary>
        /// <param name="invocationState">Состояние вызова.</param>
        public AsyncInvocationStateSyncAdapter(IInvocationState<TPipeline> invocationState)
        {
            _invocationState = invocationState;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync<TResult>(TPipeline pipeline)
        {
            return MakeTransitionAsync(pipeline);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<AsyncInvocationState<TPipeline>> MakeTransitionAsync(TPipeline pipeline)
        {
            var nextState = _invocationState.MakeTransition(pipeline);
            
            if (nextState == null)
                return Task.FromResult<AsyncInvocationState<TPipeline>>(result: null);
                      
            var resultState = nextState is MethodCallState<TPipeline> mcs  
                // ReSharper disable once RedundantCast
                ? (AsyncInvocationState<TPipeline>) new AsyncMethodCallState<TPipeline>(mcs.BoundaryAspects, mcs.InterceptionAspect)
                // ReSharper disable once RedundantCast
                : (AsyncInvocationState<TPipeline>) new AsyncInvocationStateSyncAdapter<TPipeline>(nextState);

            return Task.FromResult(resultState);
        }
    }
}