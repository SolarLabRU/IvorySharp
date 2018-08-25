using System;
using System.Reflection;
using IvorySharp.Core;

namespace IvorySharp.Tests.Assets.Invocations
{
    internal class BypassInvocation : AbstractInvocation
    {
        internal BypassInvocation(
            Type declaringType,
            object target,
            MethodInfo method,
            InvocationArguments arguments)
            : base(
                arguments,
                method,
                declaringType,
                target.GetType(),
                null,
                target)
        {
        }
        
        internal BypassInvocation(
            Type declaringType,
            object target,
            MethodInfo method)
            : base(
                InvocationArguments.Empty, 
                method,
                declaringType,
                target.GetType(),
                null,
                target)
        {
        }
        
        internal BypassInvocation(
            Type declaringType,
            object target,
            string methodName,
            InvocationArguments arguments)
            : base(
                arguments,
                declaringType.GetMethod(methodName),
                declaringType,
                target.GetType(),
                null,
                target)
        {
        }
        
        internal BypassInvocation(
            Type declaringType,
            object target,
            string methodName)
            : base(
                InvocationArguments.Empty, 
                declaringType.GetMethod(methodName),
                declaringType,
                target.GetType(),
                null,
                target)
        {
        }

        /// <inheritdoc />
        public override object ReturnValue { get; set; }

        /// <inheritdoc />
        public override object Proceed()
        {
            ReturnValue = Method.Invoke(Target, Arguments);
            return ReturnValue;
        }
    }
}