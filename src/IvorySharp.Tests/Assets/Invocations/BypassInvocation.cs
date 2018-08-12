using System;
using System.Reflection;
using IvorySharp.Core;

namespace IvorySharp.Tests.Assets.Invocations
{
    public class BypassInvocation : Invocation
    {
        public BypassInvocation(Type declaringType, object instance, string methodName)
            : base(CreateContext(declaringType, instance, declaringType.GetMethod(methodName), Array.Empty<object>()))
        {
        }
        
        public BypassInvocation(Type declaringType, object instance, MethodInfo method)
            : base(CreateContext(declaringType, instance, method, Array.Empty<object>()))
        {
        }
        
        public BypassInvocation(Type declaringType, object instance, string methodName, object[] arguments)
            : base(CreateContext(declaringType, instance, declaringType.GetMethod(methodName), arguments))
        {
        }

        public override object Proceed()
        {
            ReturnValue = Context.Method.Invoke(Context.Instance, (object[])Context.Arguments);
            return ReturnValue;
        }

        private static InvocationContext CreateContext(Type declaringType, object instance, MethodInfo method, object[] arguments)
        {
            return new InvocationContext(
                arguments, 
                method,
                instance, 
                instance, 
                declaringType,
                instance.GetType());
        }
        
    }
}