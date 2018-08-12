using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IvorySharp.Aspects.Pipeline.Appliers;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Pipeline.Async
{
    /// <summary>
    /// Компонент, выполняющий пайплайн <see cref="AsyncAspectInvocationPipeline"/>.
    /// </summary>
    internal class AsyncAspectInvocationPipelineExecutor : IInvocationPipelineExecutor
    {
        private readonly ConcurrentDictionary<Type, Func<object, object[], object>> _handlersCache;

        /// <summary>
        /// Инициализированный экземпляр <see cref="AsyncAspectInvocationPipelineExecutor"/>.
        /// </summary>
        internal static readonly AsyncAspectInvocationPipelineExecutor Instance
            = new AsyncAspectInvocationPipelineExecutor();

        private AsyncAspectInvocationPipelineExecutor()
        {
            _handlersCache = new ConcurrentDictionary<Type, Func<object, object[], object>>();
        }

        /// <inheritdoc />
        public void ExecutePipeline(IInvocationPipeline basePipeline)
        {
            // Это нарушает solid, но позволяет не выставлять кучу классов наружу библиотеки.
            var pipeline = (AsyncAspectInvocationPipeline) basePipeline;

            switch (pipeline.Invocation.InvocationType)
            {
                case InvocationType.AsyncAction:
                    pipeline.Invocation.ReturnValue = SignalWhenAwait(pipeline);
                    break;

                case InvocationType.AsyncFunction:
                    var signalWhenAwait = GetAsyncFunctionHandler(pipeline.Invocation);
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
        internal async Task SignalWhenAwait(AsyncAspectInvocationPipeline pipeline)
        {
            var aspectReducer = new MethodAspectReducer(pipeline);
            var applyResult = new AspectApplyResult();

            try
            {
                applyResult = aspectReducer.Reduce(pipeline.BoundaryAspects, OnEntryApplier.Instance);

                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                    await ((Task) pipeline.Invocation.ReturnValue)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                else
                {
                    pipeline.Invocation.ReturnValue = pipeline.IsExceptional()
                        ? Task.FromException(pipeline.CurrentException)
                        : Task.CompletedTask;
                }

                aspectReducer.ReduceBefore(pipeline.BoundaryAspects, OnSuccessApplier.Instance,
                    applyResult.ExecutionBreaker);
            }
            catch (Exception e)
            {
                applyResult = ExecuteExceptionBlock(e, pipeline, aspectReducer, applyResult.ExecutionBreaker);
            }
            finally
            {
                ExecuteFinallyBlock(pipeline, aspectReducer, applyResult.ExecutionBreaker);
            }
        }

        /// <summary>
        /// Метод обратного вызова, который выполнится, когда на исходном перехватываемом
        /// методе вызовут await.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна.</param>
        /// <typeparam name="T">Тип возвращаемых данных.</typeparam>
        /// <returns>Результат выполнения метода.</returns>
        internal async Task<T> SignalWhenAwait<T>(AsyncAspectInvocationPipeline pipeline)
        {
            var aspectReducer = new MethodAspectReducer(pipeline);
            var applyResult = new AspectApplyResult();

            try
            {
                applyResult = aspectReducer.Reduce(pipeline.BoundaryAspects, OnEntryApplier.Instance);

                // Перехватываем метод только при нормальном выполнении
                // пайплайна
                if (pipeline.FlowBehavior == FlowBehavior.Continue)
                {
                    pipeline.InterceptionAspect.OnInvoke(pipeline.Invocation);
                    pipeline.CurrentReturnValue = await ((Task<T>) pipeline.Invocation.ReturnValue)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                // Нужно пересобрать возвращаемый результат, т.к. метод
                // мы не выполнили, а клиент ждет что-то, на чем можно вызвать await
                else
                {
                    pipeline.Invocation.ReturnValue = pipeline.IsExceptional()
                        ? Task.FromException<T>(pipeline.CurrentException)
                        : Task.FromResult((T) pipeline.CurrentReturnValue);

                    // Здесь можно было бы вызвать await и выкинуть исключение сразу
                    // в catch, но в этом нет смысла, т.к. OnSuccess не вызовется на пайплайн
                    // в ошибочном состоянии.
                }

                aspectReducer.ReduceBefore(pipeline.BoundaryAspects, OnSuccessApplier.Instance,
                    applyResult.ExecutionBreaker);
            }
            catch (Exception e)
            {
                applyResult = ExecuteExceptionBlock(e, pipeline, aspectReducer, applyResult.ExecutionBreaker);
            }
            finally
            {
                ExecuteFinallyBlock(pipeline, aspectReducer, applyResult.ExecutionBreaker);
            }

            // Нужно заменить результат у клиента, если он изменился
            return pipeline.IsReturn()
                ? await Task.FromResult((T) pipeline.CurrentReturnValue)
                    .ConfigureAwait(continueOnCapturedContext: false)
                : (T) pipeline.CurrentReturnValue;
        }

        private static AspectApplyResult ExecuteExceptionBlock(
            Exception exception,
            AsyncAspectInvocationPipeline pipeline,
            MethodAspectReducer aspectReducer,
            MethodBoundaryAspect executionBreaker)
        {
            // Если это исключение, сгенерированное каким-то из обработчиков
            // прокидываем его дальше 
            if (pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                pipeline.FlowBehavior == FlowBehavior.Faulted)
            {
                exception.Throw();
            }

            // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
            pipeline.CurrentException = exception.GetInnerIf(exception is TargetInvocationException &&
                                                             exception.InnerException != null);

            // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
            pipeline.FlowBehavior = FlowBehavior.RethrowException;

            var applyResult = aspectReducer.ReduceBefore(pipeline.BoundaryAspects,
                OnExceptionApplier.Instance, executionBreaker);

            var breaker = applyResult.ExecutionBreaker;

            // Если один из обработчиков решил вернуть результат вместо исключения
            // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
            if (breaker != null && pipeline.IsReturn())
            {
                var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                    .TakeWhile(a => !Equals(a, breaker))
                    .ToArray();

                aspectReducer.Reduce(onSuccessAspects, OnSuccessApplier.Instance);
            }

            // Если никто не смог обработать исключение или в процессе обработки
            // появилось новое исключение - выбрасываем его наружу.
            if (pipeline.IsExceptional())
                pipeline.CurrentException.Throw();

            return applyResult;
        }

        private static void ExecuteFinallyBlock(
            AsyncAspectInvocationPipeline pipeline,
            MethodAspectReducer aspectReducer,
            MethodBoundaryAspect executionBreaker)
        {
            // Ничего не должно выполняться, если пайплайн в сломанном состоянии.
            if (pipeline.IsFaulted())
                pipeline.CurrentException.Throw();

            aspectReducer.ReduceBefore(pipeline.BoundaryAspects, OnExitApplier.Instance,
                executionBreaker);

            // Выкидываем исключение, если пайплайн в ошибочном состоянии
            if (pipeline.IsExceptional())
                pipeline.CurrentException.Throw();
        }


        /// <summary>
        /// Возвращает хендлер для создания продолжения вызова с использованием <see cref="SignalWhenAwait{T}"/>
        /// с внутренним типом задачи, возвращаемой методом <see cref="IInvocationContext.Method"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Хендлер для создания продолжения вызова.</returns>
        private Func<object, object[], object> GetAsyncFunctionHandler(IInvocationContext invocation)
        {
            var innerType = invocation.Method.ReturnType.GetGenericArguments()[0];

            return _handlersCache.GetOrAdd(innerType, key =>
            {
                var method = typeof(AsyncAspectInvocationPipelineExecutor)
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .First(m => m.IsGenericMethodDefinition && m.Name == nameof(SignalWhenAwait))
                    .MakeGenericMethod(innerType);

                return Expressions.CreateLambda(method);
            });
        }
    }
}