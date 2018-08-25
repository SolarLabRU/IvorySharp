using System;
using System.ComponentModel;
using System.Reflection;
using IvorySharp.Exceptions;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Модель выполнения метода.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class Invocation : AbstractInvocation
    {
        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        private readonly MethodLambda _invoker;

        /// <inheritdoc />
        public override object ReturnValue
        {
            get => _unsafeReturnValue;
            set => _unsafeReturnValue = ConvertReturnValue(value);
        }
        
        private object _unsafeReturnValue;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        internal Invocation(
            InvocationArguments arguments,
            MethodInfo proxiedMethod, 
            Type declaringType,
            Type targetType, 
            object proxy, 
            object target,
            MethodLambda invoker) 
            : base(
                arguments,
                proxiedMethod,
                declaringType,
                targetType, 
                proxy, 
                target)
        {
            _invoker = invoker;
        }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        internal Invocation(
            IInvocationSignature signature,
            InvocationArguments arguments,
            object proxy,
            object target,
            MethodLambda invoker)
            : base(
                arguments,
                signature.Method,
                signature.DeclaringType,
                signature.TargetType,
                proxy,
                target)
        {
            _invoker = invoker;
        }
        
        /// <inheritdoc />
        public override object Proceed()
        {
            try
            {
                _unsafeReturnValue = _invoker != null 
                    ? _invoker(Target, Arguments) 
                    : Method.Invoke(Target, Arguments);
                
                if (ReferenceEquals(_unsafeReturnValue, Target))
                    _unsafeReturnValue = Proxy;

                return _unsafeReturnValue;
            }
            catch (TargetInvocationException e)
            {
                e.GetInner().Throw();
                throw;
            }
        }
        
        /// <summary>
        /// Конвертирует возвращаемое значение вызова.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        public object ConvertReturnValue(object returnValue)
        {
            if (Method.IsVoidReturn())
            {
                throw new IvorySharpException(
                    $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                    $"Метод '{Method.Name}' типа '{DeclaringType.FullName}' " +
                    "не имеет возвращаемого значения (void).");
            }

            if (returnValue == null)
                return Method.ReturnType.GetDefaultValue();
            
            if (!TypeConversion.TryConvert(returnValue, Method.ReturnType, out var converted))
            {
                throw new IvorySharpException(
                    $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                    $"Тип результата '{returnValue.GetType().FullName}' " +
                    $"невозможно привести к возвращаемому типу '{Method.ReturnType.FullName}'.");
            }

            return converted;
        }
    }
}