using System;
using System.Reflection;
using IvorySharp.Exceptions;
using IvorySharp.Extensions;
using IvorySharp.Linq;
using IvorySharp.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Represents concrete method invocation.
    /// </summary>
    internal sealed class Invocation : AbstractInvocation
    {
        /// <summary>
        /// Fast method call action.
        /// </summary>
        private readonly MethodCall _invoker;

        /// <inheritdoc />
        public override object ReturnValue
        {
            get => _unsafeReturnValue;
            set => _unsafeReturnValue = ConvertReturnValue(value);
        }
        
        private object _unsafeReturnValue;

        /// <summary>
        /// Creates instance of <see cref="Invocation"/>.
        /// </summary>
        internal Invocation(
            InvocationArguments arguments,
            MethodInfo proxiedMethod, 
            Type declaringType,
            Type targetType, 
            object proxy, 
            object target,
            MethodCall invoker) 
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
        /// Creates instance of <see cref="Invocation"/>.
        /// </summary>
        internal Invocation(
            IInvocationSignature signature,
            InvocationArguments arguments,
            object proxy,
            object target,
            MethodCall invoker)
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
        /// Converts the <paramref name="returnValue"/> to expected return value of underlying method (<see cref="AbstractInvocation.Method"/>).
        /// </summary>
        /// <param name="returnValue">Expected return value.</param>
        /// <returns>Converted return value.</returns>
        public object ConvertReturnValue(object returnValue)
        {
            if (Method.IsVoidReturn())
            {
                throw new IvorySharpException(
                    $"Unable to set the return value '{returnValue}'. " +
                    $"The method '{Method.Name}' of type '{DeclaringType.FullName}' " +
                    "has no return value (returns void).");
            }

            if (returnValue == null)
                return Method.ReturnType.GetDefaultValue();
            
            if (!TypeConversion.TryConvert(returnValue, Method.ReturnType, out var converted))
            {
                throw new IvorySharpException(
                    $"Unable to set the return value '{returnValue}'. " +
                    $"Unable to cast from type '{returnValue.GetType().FullName}' " +
                    $"to method expected return type '{Method.ReturnType.FullName}'.");
            }

            return converted;
        }
    }
}