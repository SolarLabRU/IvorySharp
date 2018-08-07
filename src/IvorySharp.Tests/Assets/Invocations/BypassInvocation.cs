using System;
using IvorySharp.Core;

namespace IvorySharp.Tests.Assets
{
    public class BypassInvocation : Invocation
    {
        public BypassInvocation(Type declaringType, object instance, string methodName)
            : base(CreateContext(declaringType, instance, methodName, Array.Empty<object>()))
        {
        }
        
        public BypassInvocation(Type declaringType, object instance, string methodName, object[] arguments)
            : base(CreateContext(declaringType, instance, methodName, arguments))
        {
        }

        public override object Proceed()
        {
            return Context.Method.Invoke(Context.Instance, (object[])Context.Arguments);
        }

        private static InvocationContext CreateContext(Type declaringType, object instance, string methodName, object[] arguments)
        {
            return new InvocationContext(
                arguments, 
                declaringType.GetMethod(methodName), 
                instance, instance, declaringType, instance.GetType());
        }
    }
}