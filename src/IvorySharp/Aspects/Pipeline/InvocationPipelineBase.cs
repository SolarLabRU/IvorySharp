using System;
using System.Collections.Concurrent;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Базовая модель пайплайна выполнения метода.
    /// </summary>
    internal abstract class InvocationPipelineBase : IInvocationPipeline
    {
        private static readonly object SyncRoot = new object();
        private readonly ConcurrentDictionary<Guid, object> _pipelineData;
        
        /// <summary>
        /// Модель вызова метода.
        /// </summary>
        internal IInvocation Invocation { get; }

        /// <summary>
        /// Текущий выполняемый аспект.
        /// </summary>
        internal MethodAspect CurrentExecutingAspect { get; set; }

        /// <inheritdoc />
        public IInvocationContext Context => Invocation;

        /// <inheritdoc />
        public Exception CurrentException { get; set; }

        /// <inheritdoc />
        public abstract object CurrentReturnValue { get; set; }

        /// <inheritdoc />
        public FlowBehavior FlowBehavior { get; set; }

        /// <inheritdoc />
        public object AspectExecutionState {
            get => GetAspectState();
            set => SetAspectState(value);
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationPipelineBase"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        protected InvocationPipelineBase(IInvocation invocation)
        {
            _pipelineData = new ConcurrentDictionary<Guid, object>();
            Invocation = invocation;
        }

        /// <inheritdoc />
        public abstract void Return();

        /// <inheritdoc />
        public abstract void ReturnValue(object returnValue);

        /// <inheritdoc />
        public void ThrowException(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.ThrowException;
        }

        /// <inheritdoc />
        public void RethrowException(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.RethrowException;
        }
        
        /// <summary>
        /// Переводит пайплайн в состояние <see cref="Pipeline.FlowBehavior.Faulted"/>.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        internal void Fault(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.Faulted;
        }
        
        /// <summary>
        /// Возвращает текущее состояние аспекта.
        /// </summary>
        /// <returns>Состояние аспекта.</returns>
        private object GetAspectState()
        {
            lock (SyncRoot)
            {
                if (CurrentExecutingAspect == null)
                    return null;
                
                return _pipelineData.TryGetValue(CurrentExecutingAspect.InternalId, out var data)
                    ? data 
                    : null;
            }
        }

        /// <summary>
        /// Устанавливает текущее состояние аспекта.
        /// </summary>
        /// <param name="newState">Новое состояние.</param>
        protected void SetAspectState(object newState)
        {
            lock (SyncRoot)
            {
                if (CurrentExecutingAspect == null)
                    return;

                _pipelineData[CurrentExecutingAspect.InternalId] = newState;
            }
        }
    }
}