using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Linq;

namespace IvorySharp.Extensions
{
    /// <summary>
    /// A set of extenstion methods for <see cref="Type"/>.
    /// </summary>
    internal static class TypeExtensions
    {
        private static readonly Type[] NotInterceptableTypes =
            typeof(MethodAspect).Assembly.GetTypes().Where(t => t.IsInterface).ToArray();

        internal static object GetDefaultValue(this Type type)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            return type == typeof(void) ? null : Expressions.CreateDefaultValueGenerator(type)();
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