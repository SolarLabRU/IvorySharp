using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Reflection;

namespace IvorySharp.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly Type[] NotInterceptableTypes =
            typeof(MethodAspect).Assembly.GetTypes().Where(t => t.IsInterface).ToArray();

        internal static object GetDefaultValue(this Type type)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(void))
                return null;

            return Expressions.CreateDefaultValueGenerator(type)();
        }

        internal static IEnumerable<Type> GetInterceptableInterfaces(this Type type)
        {
            return type.GetInterfaces().Where(i => !NotInterceptableTypes.Contains(i));
        }

        internal static bool IsInterceptable(this Type type)
        {
            return !NotInterceptableTypes.Contains(type);
        }
    }
}