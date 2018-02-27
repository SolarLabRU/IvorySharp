using IvoryProxy.Core;
using IvoryProxy.Core.Attributes;

namespace IvoryProxy.Extensions
{
    internal static class IMethodInvocationExtensions
    {
        public static IMethodPreExecutionContext ToPreExecutionContext(this IInvocation invocation)
        {
            if (invocation is Invocation mi)
                return mi;

            return new Invocation(invocation.InvocationTarget, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
        }

        public static IMethodPostExecutionContext ToPostExecutionContext(this IInvocation invocation)
        {
            if (invocation is Invocation mi)
                return mi;

            var baseInvocation = new Invocation(invocation.InvocationTarget, invocation.Arguments, invocation.TargetMethod, invocation.DeclaringType);
            invocation.ReturnValue = baseInvocation.ReturnValue;

            return baseInvocation;
        }

        public static bool IsInterceptionDisallowed(this IInvocation invocation)
        {
            return invocation.DeclaringType.HasAttribute<DisallowInterceptAttribute>(inherit: false) ||
                   invocation.TargetMethod.HasAttribte<DisallowInterceptAttribute>(inherit: false);
        }

        public static bool IsVoidResult(this IInvocation invocation)
        {
            return invocation.TargetMethod.ReturnType == typeof(void);
        }
    }
}