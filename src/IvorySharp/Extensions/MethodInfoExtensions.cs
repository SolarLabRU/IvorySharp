using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IvorySharp.Comparers;
using IvorySharp.Core;

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

        internal static bool IsAsync(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes<AsyncStateMachineAttribute>(inherit: false).Any();
        }
        
        internal static MethodType GetMethodType(this MethodInfo method)
        {
            var returnType = method.ReturnType;

            if (returnType == typeof(void) ||
                !typeof(Task).IsAssignableFrom(returnType))
            {
                return MethodType.Synchronous;
            }

            return returnType.GetTypeInfo().IsGenericType 
                ? MethodType.AsyncFunction 
                : MethodType.AsyncAction;
        }
    }
}