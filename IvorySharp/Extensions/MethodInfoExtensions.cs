using System.Reflection;

namespace IvorySharp.Extensions
{
    internal static class MethodInfoExtensions
    {
        internal static bool IsVoidReturn(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType == typeof(void);
        }
    }
}