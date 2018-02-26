using IvoryProxy.Core;

namespace IvoryProxy.Tests.Interceptors
{
    public class IncrementResultInterceptor : IInterceptor
    {
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.ReturnValue is int intResult)
            {
                invocation.ReturnValue = intResult + 1;
            }
        }

        /// <inheritdoc />
        public bool CanIntercept(IMethodInvocation invocation)
        {
            return invocation.TargetMethod.ReturnType == typeof(int);
        }
    }
}