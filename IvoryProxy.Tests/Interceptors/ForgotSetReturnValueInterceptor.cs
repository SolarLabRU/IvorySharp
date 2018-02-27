using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class ForgotSetReturnValueInterceptor : IvoryInterceptor
    {
        public override void Intercept(IInvocation invocation)
        {
            //throw new System.NotImplementedException();
        }

        public bool CanIntercept(IInvocation invocation)
        {
            return true;
        }
    }
}