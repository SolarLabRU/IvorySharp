using System.Collections.Generic;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Pipeline.Async
{
    /// <summary>
    /// Модель пайлпайна выполнения асинхронного метода.
    /// </summary>
    internal sealed class AsyncInvocationPipeline : InvocationPipelineBase
    {
        /// <inheritdoc />
        public override object CurrentReturnValue { get; set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationPipeline"/>.
        /// </summary>
        public AsyncInvocationPipeline() 
        {
        }
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationPipeline"/>.
        /// </summary>
        public AsyncInvocationPipeline(
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect) 
            : base(boundaryAspects, interceptionAspect)
        {
        }

        /// <inheritdoc />
        public override void Return()
        {
            base.Return();

            if (Context.InvocationType == InvocationType.AsyncAction)
                CurrentReturnValue = null;

            var innerType = Context.Method.ReturnType.GetGenericArguments()[0];
            CurrentReturnValue = innerType.GetDefaultValue();
        }

        /// <inheritdoc />
        public override void ReturnValue(object returnValue)
        {
            base.ReturnValue(returnValue);
            
            if (Context.InvocationType == InvocationType.AsyncAction)
                return;
            
            var innerType = Context.Method.ReturnType.GetGenericArguments()[0];
            if (returnValue == null)
            {
                CurrentReturnValue = innerType.GetDefaultValue();
                return;
            }
            
            if (TypeConversion.TryConvert(returnValue, innerType, out var converted))
            {                   
                CurrentReturnValue = converted;
            }
            else
            {
                var message = $"Невозможно установить возвращаемое значение '{returnValue}', " +
                              $"т.к. его тип '{returnValue.GetType()}' неприводим " +
                              $"к ожидаемому возвращаемому типу '{innerType}'";

                throw new IvorySharpException(message);
            }
        }
    }
}