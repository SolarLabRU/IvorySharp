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

            return new MethodInvocation(invocation.InvocationTarget, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
        }

        public static IMethodPostExecutionContext ToPostExecutionContext(this IMethodInvocation invocation)
        {
            if (invocation is MethodInvocation mi)
                return mi;

            var baseInvocation = new MethodInvocation(invocation.InvocationTarget, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
            invocation.ReturnValue = baseInvocation.ReturnValue;

            return baseInvocation;
        }

        public static bool IsInterceptDisallowed(this IMethodInvocation invocation)
        {
            return invocation.DeclaringType.HasAttribute<DisallowInterceptAttribute>(inherit: false) ||
                   invocation.TargetMethod.HasAttribte<DisallowInterceptAttribute>(inherit: false);
        }

        public static bool IsVoidResult(this IMethodInvocation invocation)
        {
            return invocation.TargetMethod.ReturnType == typeof(void);
        }
    }
}