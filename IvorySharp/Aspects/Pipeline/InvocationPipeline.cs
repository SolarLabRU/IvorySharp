using System;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Реализация пайплайна выполнения метода.
    /// </summary>
    internal class InvocationPipeline : IInvocationPipeline
    {
        public InvocationPipeline(InvocationContext invocationContext)
        {
            InvocationContext = invocationContext;
        }

        /// <inheritdoc />
        public InvocationContext InvocationContext { get; }

        /// <inheritdoc />
        public Exception CurrentException { get; set; }

        /// <inheritdoc />
        public FlowBehaviour FlowBehaviour { get; set; }

        /// <inheritdoc />
        public void Return(object returnValue)
        {
            CurrentException = null;
            InvocationContext.ReturnValue = returnValue;
            FlowBehaviour = FlowBehaviour.Return;
        }

        /// <inheritdoc />
        public void Return()
        {
            FlowBehaviour = FlowBehaviour.Return;
        }

        /// <inheritdoc />
        public void ThrowException(Exception exception)
        {
            CurrentException = exception;
            FlowBehaviour = FlowBehaviour.ThrowException;
        }
    }
}