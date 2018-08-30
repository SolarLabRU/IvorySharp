using IvorySharp.Aspects;
using IvorySharp.Core;

namespace IvorySharp.Benchmark.Aspects
{
    public class BypassInterceptionAspect : MethodInterceptionAspect
    {
        public override void OnInvoke(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}