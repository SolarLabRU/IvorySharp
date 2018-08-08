using IvorySharp.Core;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Invocations;
using Xunit;

namespace IvorySharp.Tests.Asserts
{
    public class InvocationAssert
    {
        public static void ProceedCalled(IInvocation invocation)
        {
            Assert.IsType<ObservableInvocation>(invocation);
            Assert.True(invocation is ObservableInvocation oi && oi.IsProceedCalled);
        }

        public static void ProceedNotCalled(IInvocation invocation)
        {
            Assert.IsType<ObservableInvocation>(invocation);
            Assert.True(invocation is ObservableInvocation oi && !oi.IsProceedCalled);
        }
    }
}