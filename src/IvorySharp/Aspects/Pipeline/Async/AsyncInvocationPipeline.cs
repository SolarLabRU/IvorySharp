using System;
using System.Collections.Generic;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using IvorySharp.Linq;
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
        internal override Lazy<DefaultValueGenerator> DefaultReturnValueGeneratorProvider { get; }

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
            DefaultReturnValueGeneratorProvider = new Lazy<DefaultValueGenerator>(() =>
            {
                return ReturnTypeInner != null
                    ? Expressions.CreateDefaultValueGenerator(ReturnTypeInner)
                    : () => null;
            });
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

        /// <inheritdoc />
        protected override void SetDefaultReturnValue()
        {
            CurrentReturnValue = CanReturnValue
                ? DefaultReturnValueGenerator()
                : null;
        }
    }
}