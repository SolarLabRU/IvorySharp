using IvoryProxy.Core;
using IvoryProxy.Core.Attributes;

namespace IvoryProxy.Extensions
{
    internal static class IMethodInvocationExtensions
    {
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