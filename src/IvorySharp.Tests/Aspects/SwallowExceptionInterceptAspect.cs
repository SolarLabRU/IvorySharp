using System;
using IvorySharp.Aspects;
using IvorySharp.Core;

namespace IvorySharp.Tests.Aspects
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwallowExceptionInterceptAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                invocation.Context.ReturnValue = default(object);
            }
        }
    }
}