using System;
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
        internal override bool CanReturnValue { get; }

        /// <inheritdoc />
        internal override Lazy<object> DefaultReturnValueProvider { get; }
        
        /// <summary>
        /// Внутренний тип возвращаемого значения.
        /// Устанавливается только если выполняется асинхронная функция (Task{T}).
        /// </summary>
        internal Type ReturnTypeInner { get; }

        /// <inheritdoc />
        public override object CurrentReturnValue { get; set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AsyncInvocationPipeline"/>.
        /// </summary>
        public AsyncInvocationPipeline(
            IInvocationSignature signature,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects,
            MethodInterceptionAspect interceptionAspect)
            : base(signature, boundaryAspects, interceptionAspect)
        {
            ReturnTypeInner = signature.InvocationType == InvocationType.AsyncFunction
                ? signature.Method.ReturnType.GetGenericArguments()[0]
                : null;

            CanReturnValue = ReturnTypeInner != null;
            DefaultReturnValueProvider = new Lazy<object>(() => ReturnTypeInner != null
                ? ReturnTypeInner.GetDefaultValue()
                : null);
        }

        /// <inheritdoc />
        internal override void ResetState()
        {
            SetDefaultReturnValue();
            base.ResetState();
        }

        /// <inheritdoc />
        protected override void SetReturnValue(object returnValue)
        {
            // Метод возвращает Task
            if (!CanReturnValue)
            {
                CurrentReturnValue = null;
            }
            else
            {
                // Значение не указано
                if (returnValue == null)
                {
                    CurrentReturnValue = DefaultReturnValueProvider.Value;
                    return;
                }
                
                if (TypeConversion.TryConvert(returnValue, ReturnTypeInner, out var converted))
                {
                    CurrentReturnValue = converted;
                }
                else
                {
                    var message = $"Невозможно установить возвращаемое значение '{returnValue}', " +
                                  $"т.к. его тип '{returnValue.GetType()}' неприводим " +
                                  $"к ожидаемому возвращаемому типу '{ReturnTypeInner}'";

                    throw new IvorySharpException(message);
                }
            }
        }

        /// <inheritdoc />
        protected override void SetDefaultReturnValue()
        {
            CurrentReturnValue = CanReturnValue
                ? DefaultReturnValueProvider.Value
                : null;
        }
    }
}