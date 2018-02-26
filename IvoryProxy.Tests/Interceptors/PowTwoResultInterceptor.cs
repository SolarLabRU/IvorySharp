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
                invocation.TrySetReturnValue(intResult * intResult);
            }
        }

        public bool CanIntercept(IMethodPreExecutionContext context)
        {
            return context.TargetMethod.ReturnType == typeof(int);
        }
    }
}