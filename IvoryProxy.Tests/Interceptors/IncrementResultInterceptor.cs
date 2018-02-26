using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class IncrementResultInterceptor : IMethodInterceptor
    {
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            var result = invocation.TargetMethod.Invoke(invocation.Target, invocation.Arguments);
            if (result is int intResult)
            {
                result = intResult + 1;
            }

            invocation.TrySetReturnValue(result);
        }
    }
}