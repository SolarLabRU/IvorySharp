using IvoryProxy.Core;

namespace IvoryProxy.Extensions
{
    internal static class IMethodInvocationExtensions
    {
        public static IMethodPreExecutionContext ToPreExecutionContext(this IMethodInvocation invocation)
        {
            if (invocation is MethodInvocation mi)
                return mi;

            return new MethodInvocation(invocation.Target, invocation.Arguments, invocation.TargetMethod);
        }

        public static IMethodPostExecutionContext ToPostExecutionContext(this IMethodInvocation invocation)
        {
            if (invocation is MethodInvocation mi)
                return mi;

            var baseInvocation = new MethodInvocation(invocation.Target, invocation.Arguments, invocation.TargetMethod);
            baseInvocation.TrySetReturnValue(invocation.ReturnValue);

            return baseInvocation;
        }
    }
}