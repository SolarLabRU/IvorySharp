using IvoryProxy.Core;
using IvoryProxy.Core.Attributes;

namespace IvoryProxy.Extensions
{
    internal static class IMethodInvocationExtensions
    {
        public static IMethodPreExecutionContext ToPreExecutionContext(this IMethodInvocation invocation)
        {
            if (invocation is MethodInvocation mi)
                return mi;

            return new MethodInvocation(invocation.Target, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
        }

        public static IMethodPostExecutionContext ToPostExecutionContext(this IMethodInvocation invocation)
        {
            if (invocation is MethodInvocation mi)
                return mi;

            var baseInvocation = new MethodInvocation(invocation.Target, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
            baseInvocation.TrySetReturnValue(invocation.ReturnValue);

            return baseInvocation;
        }

        public static bool IsInterceptDisallowed(this IMethodInvocation invocation)
        {
            return invocation.Target.GetType().HasAttribute<DisallowInterceptAttribute>(inherit: false) ||
                   invocation.TargetMethod.HasAttribte<DisallowInterceptAttribute>(inherit: false);
        }
    }
}