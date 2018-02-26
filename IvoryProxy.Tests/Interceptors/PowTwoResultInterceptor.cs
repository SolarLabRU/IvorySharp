using IvoryProxy.Core;

namespace IvoryProxy.Tests.Interceptors
{
    public class PowTwoResultInterceptor : IInterceptor
    {
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.ReturnValue is int intResult)
            {
                invocation.ReturnValue = intResult * intResult;
            }
        }

        public bool CanIntercept(IMethodInvocation invocation)
        {
            return invocation.TargetMethod.ReturnType == typeof(int);
        }
    }
}