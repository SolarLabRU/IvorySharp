using System;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline.StateMachine;

namespace IvorySharp.Aspects.Pipeline.Async.StateMachine
{
    /// <summary>
    /// Аналогично <see cref="InvocationStateMachine{TPipeline}"/>, только для асинхронных методов.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal sealed class AsyncInvocationStateMachine<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        private readonly TPipeline _pipeline;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationStateMachine{TPipeline}"/>.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна.</param>
        public AsyncInvocationStateMachine(TPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        /// <summary>
        /// Инициирует выполнение стейт-машины обработки пайплайна вызова метода.
        /// Вызывается для методов, возвращающих <see cref="Task{TResult}"/>.
        /// </summary>
        /// <param name="initialState">Начальное состояние.</param>
        /// <typeparam name="TResult">Тип возвращаемого результата.</typeparam>
        /// <returns>Результат вызова метода.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(AsyncInvocationState<TPipeline> initialState)
        {
            try
            {
                var nextState = await initialState.MakeTransitionAsync<TResult>(_pipeline)
                    .ConfigureAwait(continueOnCapturedContext: false);

                while (nextState != null)
                {
                    nextState = await nextState.MakeTransitionAsync<TResult>(_pipeline)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (Exception e)
            {
                _pipeline.Throw(e);
            }

            if (_pipeline.IsReturn())
                return await Task.FromResult((TResult) _pipeline.CurrentReturnValue)
                    .ConfigureAwait(continueOnCapturedContext: false);

            if (_pipeline.IsExceptional())
                return await Task.FromException<TResult>(_pipeline.CurrentException)
                    .ConfigureAwait(continueOnCapturedContext: false);

            return (TResult) _pipeline.CurrentReturnValue;
        }

        /// <summary>
        /// Инициирует выполнение стейт-машины обработки пайплайна вызова метода.
        /// Вызывается для методов, возвращающих <see cref="Task"/>.
        /// </summary>
        /// <param name="initialState">Начальное состояние.</param>
        public async Task ExecuteAsync(AsyncInvocationState<TPipeline> initialState)
        {
            var nextState = await initialState.MakeTransitionAsync(_pipeline)
                .ConfigureAwait(continueOnCapturedContext: false);

            try
            {
                while (nextState != null)
                {
                    nextState = await nextState.MakeTransitionAsync(_pipeline)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
            }
            catch (Exception e)
            {
                _pipeline.Throw(e);
            }

            if (_pipeline.IsExceptional())
                await Task.FromException(_pipeline.CurrentException)
                    .ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}