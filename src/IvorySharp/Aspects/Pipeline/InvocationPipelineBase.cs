using System;
using System.Collections.Concurrent;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Базовая модель пайплайна выполнения метода.
    /// </summary>
    internal abstract class InvocationPipelineBase : IInvocationPipeline
    {
        private static readonly object SyncRoot = new object();
        private readonly ConcurrentDictionary<Type, object> _pipelineData;
        
        /// <summary>
        /// Модель вызова метода.
        /// </summary>
        internal IInvocation Invocation { get; }

        /// <summary>
        /// Текущий выполняемый аспект.
        /// </summary>
        internal MethodAspect CurrentExecutingAspect { get; set; }

        /// <inheritdoc />
        public InvocationContext Context { get; }

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
            _pipelineData = new ConcurrentDictionary<Type, object>();
            Invocation = invocation;
            Context = Invocation.Context;
        }
        
        /// <inheritdoc />
        public void Return()
        {
            CurrentReturnValue = Context.Method.ReturnType.GetDefaultValue();
            FlowBehavior = FlowBehavior.Return;
        }

        /// <inheritdoc />
        public void ReturnValue(object returnValue)
        {
            CurrentException = null;
            FlowBehavior = FlowBehavior.Return;        
            CurrentReturnValue = returnValue;
        }

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
                
                return _pipelineData.TryGetValue(CurrentExecutingAspect.GetType(), out var data)
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

                _pipelineData[CurrentExecutingAspect.GetType()] = newState;
            }
        }
    }
}