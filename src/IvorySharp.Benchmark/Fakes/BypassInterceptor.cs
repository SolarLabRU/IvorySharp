using IvorySharp.Core;

namespace IvorySharp.Benchmark.Fakes
{
    internal class BypassInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}