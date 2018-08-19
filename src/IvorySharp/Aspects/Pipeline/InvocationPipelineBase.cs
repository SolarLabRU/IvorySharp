using System;
using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Базовая модель пайплайна выполнения метода.
    /// </summary>
    internal abstract class InvocationPipelineBase : IInvocationPipeline
    {
        private static readonly object SyncRoot = new object();
        private readonly Dictionary<Guid, object> _pipelineData;
        
        /// <summary>
        /// Аспекты типа <see cref="MethodBoundaryAspect"/>.
        /// </summary>
        internal IReadOnlyCollection<MethodBoundaryAspect> BoundaryAspects { get; }
        
        /// <summary>
        /// Аспект перехвата вызова метода.
        /// </summary>
        internal MethodInterceptionAspect InterceptionAspect { get; }
        
        /// <summary>
        /// Внутреннее состояние пайплайна.
        /// </summary>
        internal InvocationPipelineState InternalState { get; set; } 
        
        /// <summary>
        /// Модель вызова метода.
        /// </summary>
        internal IInvocation Invocation { get; }

        /// <summary>
        /// Ключ состояния вызова, для установки/получения <see cref="ExecutionState"/>.
        /// </summary>
        internal Guid? ExecutionStateKey { get; set; }
        
        /// <inheritdoc />
        public IInvocationContext Context => Invocation;

        /// <inheritdoc />
        public Exception CurrentException { get; set; }

        /// <inheritdoc />
        public abstract object CurrentReturnValue { get; set; }

        /// <inheritdoc />
        public FlowBehavior FlowBehavior { get; set; }

        /// <inheritdoc />
        public object ExecutionState {
            get => GetAspectState();
            set => SetAspectState(value);
        }

        /// <summary>
        /// Инициаилизирует экземпляр <see cref="InvocationPipelineBase"/>.
        /// </summary>
        protected InvocationPipelineBase(
            IInvocation invocation,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect)
        {
            _pipelineData = new Dictionary<Guid, object>();

            Invocation = invocation;
            BoundaryAspects = boundaryAspects;
            InterceptionAspect = interceptionAspect;
        }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationPipelineBase"/>.
        /// </summary>
        /// <param name="invocation">Модель вызова метода.</param>
        protected InvocationPipelineBase(IInvocation invocation)
           : this(invocation, Array.Empty<MethodBoundaryAspect>(), BypassMethodAspect.Instance) 
        { }

        /// <inheritdoc />
        public virtual void Return()
        {
            FlowBehavior = FlowBehavior.Return;
            InternalState = InvocationPipelineState.Return;
        }

        /// <inheritdoc />
        public virtual void ReturnValue(object returnValue)
        {
            FlowBehavior = FlowBehavior.Return;
            InternalState = InvocationPipelineState.Return;
        }

        /// <inheritdoc />
        public void ThrowException(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.ThrowException;
            InternalState = InvocationPipelineState.Exception;
        }

        /// <inheritdoc />
        public void RethrowException(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.RethrowException;
            InternalState = InvocationPipelineState.Exception;
        }
        
        /// <summary>
        /// Возвращает текущее состояние аспекта.
        /// </summary>
        /// <returns>Состояние аспекта.</returns>
        private object GetAspectState()
        {
            lock (SyncRoot)
            {
                if (ExecutionStateKey == null)
                    return null;
                
                return _pipelineData.TryGetValue(ExecutionStateKey.Value, out var data)
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
                if (ExecutionStateKey == null)
                    return;

                _pipelineData[ExecutionStateKey.Value] = newState;
            }
        }
    }
}