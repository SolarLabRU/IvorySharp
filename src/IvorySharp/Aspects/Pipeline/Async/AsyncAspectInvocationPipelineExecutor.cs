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

            switch (pipeline.Context.MethodType)
            {
                case MethodType.AsyncAction:
                    pipeline.Invocation.ReturnValue = SignalWhenAwait(pipeline);
                    break;

                case MethodType.AsyncFunction:
                    var signalWhenAwait = GetAsyncFunctionHandler(pipeline.Invocation);
                    pipeline.Invocation.ReturnValue = signalWhenAwait(this, new[] { pipeline });
                    break;

                default:
                    throw new NotSupportedException(
                        $"Поддерживание метода типа '{pipeline.Context.MethodType}' не поддерживается");
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
            var apsectReducer = new MethodAspectReducer(pipeline);
            var applyResult = new AspectApplyResult();

            try
            {
                applyResult = apsectReducer.Reduce(pipeline.BoundaryAspects, OnEntryApplier.Instance);

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

                var includeBreaker = applyResult.IsExecutionBreaked && pipeline.IsReturn();

                apsectReducer.ReduceBefore(pipeline.BoundaryAspects, OnSuccessApplier.Instance,
                    applyResult.ExecutionBreaker, includeBreaker);
            }
            catch (Exception e)
            {
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                    pipeline.FlowBehavior == FlowBehavior.Faulted)
                    throw;

                pipeline.CurrentException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);
                pipeline.FlowBehavior = FlowBehavior.RethrowException;

                applyResult = apsectReducer.ReduceBefore(pipeline.BoundaryAspects,
                    OnExceptionApplier.Instance, applyResult.ExecutionBreaker, inclusive: true);

                var breaker = applyResult.ExecutionBreaker;
                if (breaker != null && pipeline.IsReturn())
                {
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    apsectReducer.Reduce(onSuccessAspects, OnSuccessApplier.Instance);
                }

                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
            }
            finally
            {
                if (pipeline.IsFaulted())
                    pipeline.CurrentException.Throw();

                apsectReducer.ReduceBefore(pipeline.BoundaryAspects, OnExitApplier.Instance,
                    applyResult.ExecutionBreaker, inclusive: true);

                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
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
            var apsectReducer = new MethodAspectReducer(pipeline);
            var applyResult = new AspectApplyResult();

            try
            {
                applyResult = apsectReducer.Reduce(pipeline.BoundaryAspects, OnEntryApplier.Instance);

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

                // Если решили вернуть результат в OnEntry, то необходимо выполнить OnSuccess
                // так же у аспекта, решившего вернуть результат.
                var includeBreaker = applyResult.IsExecutionBreaked && pipeline.IsReturn();

                apsectReducer.ReduceBefore(pipeline.BoundaryAspects, OnSuccessApplier.Instance,
                    applyResult.ExecutionBreaker, includeBreaker);
            }
            catch (Exception e)
            {
                // Если это исключение, сгенерированное каким-то из обработчиков
                // прокидываем его дальше 
                if (pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                    pipeline.FlowBehavior == FlowBehavior.Faulted)
                    throw;

                // Устанавливаем исключение в пайплайн (распаковываем - если оно связано с рефлексией).
                pipeline.CurrentException = e.GetInnerIf(e is TargetInvocationException && e.InnerException != null);

                // Устанавливаем состояние пайплайна, при котором для каждого из обработчиков вызовется OnException
                pipeline.FlowBehavior = FlowBehavior.RethrowException;

                applyResult = apsectReducer.ReduceBefore(pipeline.BoundaryAspects,
                    OnExceptionApplier.Instance, applyResult.ExecutionBreaker, inclusive: true);

                var breaker = applyResult.ExecutionBreaker;

                // Если один из обработчиков решил вернуть результат вместо исключения
                // то мы должны позвать обработчики OnSuccess у всех родительских аспектов    
                if (breaker != null && pipeline.IsReturn())
                {
                    var onSuccessAspects = pipeline.BoundaryAspects.Reverse()
                        .TakeWhile(a => !Equals(a, breaker))
                        .ToArray();

                    apsectReducer.Reduce(onSuccessAspects, OnSuccessApplier.Instance);
                }

                // Если никто не смог обработать исключение или в процессе обработки
                // появилось новое исключение - выбрасываем его наружу.
                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
            }
            finally
            {
                // Ничего не должно выполняться, если пайплайн в сломанном состоянии.
                if (pipeline.IsFaulted())
                    pipeline.CurrentException.Throw();

                apsectReducer.ReduceBefore(pipeline.BoundaryAspects, OnExitApplier.Instance,
                    applyResult.ExecutionBreaker, inclusive: true);

                // Выкидываем исключение, если пайплайн в ошибочном состоянии
                if (pipeline.IsExceptional())
                    pipeline.CurrentException.Throw();
            }

            // Нужно заменить результат у клиента, если он изменился
            return pipeline.IsReturn()
                ? await Task.FromResult((T) pipeline.CurrentReturnValue)
                    .ConfigureAwait(continueOnCapturedContext: false)
                : (T) pipeline.CurrentReturnValue;
        }

        /// <summary>
        /// Возвращает хендлер для создания продолжения вызова с использованием <see cref="SignalWhenAwait{T}"/>
        /// с внутренним типом задачи, возвращаемой методом <see cref="InvocationContext.Method"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Хендлер для создания продолжения вызова.</returns>
        internal Func<object, object[], object> GetAsyncFunctionHandler(IInvocation invocation)
        {
            var innerType = invocation.Context.Method.ReturnType.GetGenericArguments()[0];

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