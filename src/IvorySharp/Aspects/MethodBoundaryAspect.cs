using System.Collections.Generic;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовая модель аспекта, выполняющего внедрение кода перед и после фактического выполнения метода.
    /// </summary>
    public abstract class MethodBoundaryAspect : MethodAspect, IMethodBoundaryAspect
    {
        /// <inheritdoc />
        public int Order { get; set; }
        
        /// <inheritdoc />
        public virtual void OnEntry(IInvocationPipeline pipeline)
        { }

        /// <inheritdoc />
        public virtual void OnSuccess(IInvocationPipeline pipeline)
        { }

        /// <inheritdoc />
        public virtual void OnException(IInvocationPipeline pipeline)
        { }

        /// <inheritdoc />
        public virtual void OnExit(IInvocationPipeline pipeline)
        { }
    }
}