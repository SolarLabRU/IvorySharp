using System;
using IvorySharp.Core;

namespace IvorySharp.Tests.Assets
{
    public class BypassInvocation : IInterceptableInvocation
    {
        private readonly Invocation _baseInvocation;
        
        public InvocationContext Context { get; }

        public BypassInvocation(Type declaringType, object instance, string methodName)
        {
            Context = new InvocationContext(
                Array.Empty<object>(), 
                declaringType.GetMethod(methodName), 
                instance, instance, declaringType, instance.GetType());
            
            _baseInvocation = new Invocation(Context);
        }
        
        public BypassInvocation(Type declaringType, object instance, string methodName, object[] arguments)
        {
            Context = new InvocationContext(
                arguments, 
                declaringType.GetMethod(methodName), 
                instance, instance, declaringType, instance.GetType());
        }

        public virtual object Proceed()
        {
            return Context.Method.Invoke(Context.Instance, (object[])Context.Arguments);
        }

        public void SetReturnValue(object returnValue)
        {
            _baseInvocation.SetReturnValue(returnValue);
        }
    }
}