using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class PowTwoResultInterceptor : IvoryInterceptor
    {
        /// <inheritdoc />
        public override void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.ReturnValue is int intResult)
            {
                invocation.ReturnValue = intResult * intResult;
            }
        }

        public bool CanIntercept(IInvocation invocation)
        {
            return invocation.TargetMethod.ReturnType == typeof(int);
        }
    }
}