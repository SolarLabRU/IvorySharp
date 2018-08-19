using System.Collections.Generic;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.Synchronous
{
    /// <summary>
    /// Модель пайплайна выполнения метода.
    /// </summary>
    internal sealed class InvocationPipeline : InvocationPipelineBase
    {
        /// <inheritdoc />
        public override object CurrentReturnValue
        {
            get => Invocation.ReturnValue;
            set => Invocation.ReturnValue = value;
        }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationPipeline"/>.
        /// </summary>
        public InvocationPipeline(
            IInvocation invocation,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect) 
            : base(invocation, boundaryAspects, interceptionAspect)
        {
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationPipeline"/>.
        /// </summary>
        public InvocationPipeline(IInvocation invocation) 
            : base(invocation)
        {
        }

        /// <inheritdoc />
        public override void Return()
        {
            base.Return();
            CurrentReturnValue = Context.Method.ReturnType.GetDefaultValue();
        }

        /// <inheritdoc />
        public override void ReturnValue(object returnValue)
        {
            base.ReturnValue(returnValue);      
            CurrentReturnValue = returnValue;
        }
    }
}