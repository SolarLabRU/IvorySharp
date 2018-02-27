using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class IncrementResultInterceptor : IvoryInterceptor
    {
        /// <inheritdoc />
        public override void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.ReturnValue is int intResult)
            {
                invocation.ReturnValue = intResult + 1;
            }
        }
    }
}