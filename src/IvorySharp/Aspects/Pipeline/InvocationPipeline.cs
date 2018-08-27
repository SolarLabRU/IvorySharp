using System;
using System.Collections.Generic;
using IvorySharp.Core;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Модель пайплайна выполнения метода.
    /// </summary>
    internal sealed class InvocationPipeline : InvocationPipelineBase
    {
        /// <inheritdoc />
        internal override bool CanReturnValue { get; }

        /// <inheritdoc />
        internal override Lazy<object> DefaultReturnValueProvider { get; }

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
            IInvocationSignature signature,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect) 
            : base(
                signature,
                boundaryAspects, 
                interceptionAspect)
        {
            CanReturnValue = !signature.Method.IsVoidReturn();        
            DefaultReturnValueProvider = new Lazy<object>(() => CanReturnValue
                ? signature.Method.ReturnType.GetDefaultValue()
                : null);
        }

        /// <inheritdoc />
        protected override void SetReturnValue(object returnValue)
        {
            if (Invocation != null && CanReturnValue)
                CurrentReturnValue = returnValue;
        }

        /// <inheritdoc />
        protected override void SetDefaultReturnValue()
        {
            if (Invocation != null && CanReturnValue)
                CurrentReturnValue = DefaultReturnValueProvider.Value;
        }
    }
}