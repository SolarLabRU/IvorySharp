using System;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Reflection
{
    internal static class ReflectedMethod
    {
        internal static MethodInfo GetMethodMap([NotNull] Type targetType, [NotNull] MethodInfo interfaceMethod)
        {
            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (interfaceMethod.DeclaringType == null)
                return null;
            
            var mapping = targetType.GetInterfaceMap(interfaceMethod.DeclaringType);
            var index = Array.IndexOf(mapping.InterfaceMethods, interfaceMethod);

            return index == -1 ? null : mapping.TargetMethods[index];
        }
    }
}