using System;
using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class SwallowExceptionInterceptor : IvoryInterceptor
    {
        public override void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                //
            }
            finally
            {
                var returnType = invocation.TargetMethod.ReturnType;
                invocation.ReturnValue = returnType != typeof(void) && returnType.IsValueType
                    ? Activator.CreateInstance(returnType)
                    : null;
            }
        }
    }
}