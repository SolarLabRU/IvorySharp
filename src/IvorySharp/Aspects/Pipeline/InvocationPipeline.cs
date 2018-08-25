using System.Collections.Generic;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
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
        public InvocationPipeline() 
        {
        }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationPipeline"/>.
        /// </summary>
        public InvocationPipeline(
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect) 
            : base(boundaryAspects, interceptionAspect)
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