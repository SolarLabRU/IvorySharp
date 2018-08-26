using System;
using System.Collections.Generic;
using IvorySharp.Core;
using IvorySharp.Exceptions;
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
        internal override Func<object> DefaultReturnValueGenerator { get; }

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
            DefaultReturnValueGenerator = ReturnTypeInner != null
                ? Expressions.CreateDefaultValueGenerator(ReturnTypeInner)
                : () => null;
        }

        /// <inheritdoc />
        internal override void ResetState()
        {
            CurrentReturnValue = CanReturnValue
                ? DefaultReturnValueGenerator()
                : null;

            base.ResetState();
        }

        /// <inheritdoc />
        public override void Return()
        {
            base.Return();

            if (Context.InvocationType == InvocationType.AsyncAction)
                CurrentReturnValue = null;

            CurrentReturnValue = DefaultReturnValueGenerator();
        }

        /// <inheritdoc />
        public override void ReturnValue(object returnValue)
        {
            base.ReturnValue(returnValue);

            if (Context.InvocationType == InvocationType.AsyncAction)
                return;

            if (returnValue == null)
            {
                CurrentReturnValue = DefaultReturnValueGenerator();
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
}