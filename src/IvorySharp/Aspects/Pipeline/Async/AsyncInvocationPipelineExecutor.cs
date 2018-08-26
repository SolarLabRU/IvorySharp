using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline.Async.StateMachine;
using IvorySharp.Aspects.Pipeline.StateMachine;
using IvorySharp.Caching;
using IvorySharp.Core;
using IvorySharp.Linq;

namespace IvorySharp.Aspects.Pipeline.Async
{
    /// <summary>
    /// Компонент, выполняющий пайплайн <see cref="AsyncInvocationPipeline"/>.
    /// </summary>
    internal sealed class AsyncInvocationPipelineExecutor : IInvocationPipelineExecutor
    {
        private readonly IKeyValueCache<Type, MethodLambda> _handlersCache;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationPipelineExecutor"/>.
        /// </summary>
        /// <param name="cacheFactory">Фабрика кеша.</param>
        internal AsyncInvocationPipelineExecutor(IKeyValueCacheFactory cacheFactory)
        {
            _handlersCache = cacheFactory.Create<Type, MethodLambda>();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePipeline(IInvocationPipeline basePipeline, IInvocation invocation)
        {
            // Это нарушает solid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (AsyncInvocationPipeline) basePipeline;
            pipeline.Init(invocation);

            switch (pipeline.Invocation.InvocationType)
            {
                case InvocationType.AsyncAction:
                    pipeline.Invocation.ReturnValue = SignalWhenAwait(pipeline);
                    break;

                case InvocationType.AsyncFunction:
                    var signalWhenAwait = GetAsyncFunctionHandler(_handlersCache, pipeline.Invocation);
                    pipeline.Invocation.ReturnValue = signalWhenAwait(this, new object[] { pipeline });
                    break;

                default:
                    throw new NotSupportedException(
                        $"Асинхронное выполнение метода типа '{pipeline.Invocation.InvocationType}' не поддерживается");
            }
        }

        /// <summary>
        /// Метод обратного вызова, который выполнится, когда на исходном перехватываемом
        /// методе вызовут await.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна.</param>
        /// <returns>Результат выполнения метода.</returns>
        /// 
        // ReSharper disable once MemberCanBeMadeStatic.Local
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task SignalWhenAwait(AsyncInvocationPipeline pipeline)
        {
            var stateMachine = new AsyncInvocationStateMachine<AsyncInvocationPipeline>(pipeline);

            await stateMachine.ExecuteAsync(
                new AsyncInvocationStateSyncAdapter<AsyncInvocationPipeline>(
                    new EntryState<AsyncInvocationPipeline>(pipeline.BoundaryAspects,
                        pipeline.InterceptionAspect)));
        }

        /// <summary>
        /// Метод обратного вызова, который выполнится, когда на исходном перехватываемом
        /// методе вызовут await.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна.</param>
        /// <typeparam name="T">Тип возвращаемых данных.</typeparam>
        /// <returns>Результат выполнения метода.</returns>
        ///  
        // ReSharper disable once MemberCanBeMadeStatic.Local
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<T> SignalWhenAwait<T>(AsyncInvocationPipeline pipeline)
        {
            var stateMachine = new AsyncInvocationStateMachine<AsyncInvocationPipeline>(pipeline);

            return await stateMachine.ExecuteAsync<T>(
                new AsyncInvocationStateSyncAdapter<AsyncInvocationPipeline>(
                    new EntryState<AsyncInvocationPipeline>(
                        pipeline.BoundaryAspects, pipeline.InterceptionAspect)));
        }

        /// <summary>
        /// Возвращает хендлер для создания продолжения вызова с использованием <see cref="SignalWhenAwait{T}"/>
        /// с внутренним типом задачи, возвращаемой методом <see cref="IInvocationSignature.Method"/>.
        /// </summary>
        /// <param name="cache">Кеш хендлеров.</param>
        /// <param name="signature">Модель вызова.</param>
        /// <returns>Хендлер для создания продолжения вызова.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static MethodLambda GetAsyncFunctionHandler(
            IKeyValueCache<Type, MethodLambda> cache, 
            IInvocationSignature signature)
        {
            var innerType = signature.Method.ReturnType.GetGenericArguments()[0];

            return cache.GetOrAdd(innerType, key =>
            {
                var method = typeof(AsyncInvocationPipelineExecutor)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .First(m => m.IsGenericMethodDefinition && m.Name == nameof(SignalWhenAwait))
                    .MakeGenericMethod(innerType);

                return Expressions.CreateLambda(method);
            });
        }
    }
}