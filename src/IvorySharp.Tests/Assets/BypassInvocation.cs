using System;
using IvorySharp.Core;

namespace IvorySharp.Tests.Assets
{
    public class BypassInvocation : IInvocation
    {
        public InvocationContext Context { get; }

        public BypassInvocation(Type declaringType, object instance, string methodName)
        {
            Context = new InvocationContext(Array.Empty<object>(), declaringType.GetMethod(methodName), instance, instance, declaringType, instance.GetType());
        }

        public void Proceed()
        {
            Context.Method.Invoke(Context.Instance, (object[])Context.Arguments);
        }
    }
}