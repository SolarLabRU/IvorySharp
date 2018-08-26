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
        
        private readonly Lazy<Dictionary<Guid, object>> _pipelineDataProvider;
        private Dictionary<Guid, object> PipelineData => _pipelineDataProvider.Value;
         
        /// <summary>
        /// Возвращает признак того, что из пайплайна можно вернуть значение.
        /// </summary>
        internal abstract bool CanReturnValue { get; }
        
        /// <summary>
        /// Провайдера генератора возвращаемых значений по умолчанию.
        /// </summary>
        internal abstract Lazy<Func<object>> DefaultReturnValueGeneratorProvider { get; }

        /// <summary>
        /// Генератор возвращаемых значений по умолчанию.
        /// </summary>
        internal Func<object> DefaultReturnValueGenerator => DefaultReturnValueGeneratorProvider.Value;
        
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
        internal IInvocation Invocation { get; private set; }

        /// <summary>
        /// Сигнатура вызова.
        /// </summary>
        internal IInvocationSignature InvocationSignature { get; }
        
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
            IInvocationSignature signature,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect)
        {
            _pipelineDataProvider = new Lazy<Dictionary<Guid, object>>(
                () => new Dictionary<Guid, object>());

            InvocationSignature = signature;
            BoundaryAspects = boundaryAspects;
            InterceptionAspect = interceptionAspect;
        }
        
        /// <summary>
        /// Инициализирует пайплайн.
        /// </summary>
        /// <param name="invocation">Контекст вызова.</param>
        /// <returns>Инициализированный пайплайн.</returns>
        internal InvocationPipelineBase Init(IInvocation invocation)
        {
            ResetState();
            Invocation = invocation;
            return this;
        }

        /// <summary>
        /// Сбрасывает состояние пайплайна.
        /// </summary>
        internal void ResetState()
        {       
            SetDefaultReturnValue();
            ExecutionStateKey = null;
            CurrentException = null;
            Invocation = null;
            InternalState = InvocationPipelineState.Continue;
            FlowBehavior = FlowBehavior.Continue;
        }

        /// <inheritdoc />
        public void Return()
        {
            SetDefaultReturnValue();
            FlowBehavior = FlowBehavior.Return;
            InternalState = InvocationPipelineState.Return;
        }

        /// <inheritdoc />
        public void Return(object returnValue)
        {
            SetReturnValue(returnValue);
            FlowBehavior = FlowBehavior.Return;
            InternalState = InvocationPipelineState.Return;
        }

        /// <inheritdoc />
        public void Throw(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.ThrowException;
            InternalState = InvocationPipelineState.Exception;
        }

        /// <inheritdoc />
        public void Continue(Exception exception)
        {
            CurrentException = exception ?? throw new ArgumentNullException(nameof(exception));
            FlowBehavior = FlowBehavior.RethrowException;
            InternalState = InvocationPipelineState.Exception;
        }

        /// <inheritdoc />
        public void Continue(object returnValue)
        {
            SetReturnValue(returnValue);
            FlowBehavior = FlowBehavior.UpdateReturnValue;
            InternalState = InvocationPipelineState.Continue;
        }

        /// <inheritdoc />
        public void Continue()
        {
            SetDefaultReturnValue();
            FlowBehavior = FlowBehavior.UpdateReturnValue;
            InternalState = InvocationPipelineState.Continue;
        }

        /// <summary>
        /// Устанавливает возвращаемое значение.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение,</param>
        protected abstract void SetReturnValue(object returnValue);
        
        /// <summary>
        /// Устанавливает возвращаемое значение по умолчанию.
        /// </summary>
        protected abstract void SetDefaultReturnValue();
        
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
                
                return PipelineData.TryGetValue(ExecutionStateKey.Value, out var data)
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

                PipelineData[ExecutionStateKey.Value] = newState;
            }
        }
    }
}