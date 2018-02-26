using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class ForgotSetReturnValueInterceptor : IInterceptor
    {
        public void Intercept(IMethodInvocation invocation)
        {
            //throw new System.NotImplementedException();
        }

        public bool CanIntercept(IMethodPreExecutionContext context)
        {
            return true;
        }
    }
}