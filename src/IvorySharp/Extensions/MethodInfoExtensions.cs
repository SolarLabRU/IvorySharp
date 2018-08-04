using System.Linq;
using System.Reflection;
using IvorySharp.Comparers;

namespace IvorySharp.Extensions
{
    internal static class MethodInfoExtensions
    {
        private static readonly MethodInfo[] NotInterceptableMethods = typeof(object).GetMethods();

        internal static bool IsVoidReturn(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(void);
        }

        internal static bool IsInterceptable(this MethodInfo methodInfo)
        {
            return !NotInterceptableMethods.Any(m => MethodEqualityComparer.Instance.Equals(m, methodInfo));
        }
    }
}