using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IvorySharp.Aspects;

namespace IvorySharp.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly Type[] NotInterceptableTypes =
            typeof(MethodAspect).Assembly.GetTypes().Where(t => t.IsInterface).ToArray();

        internal static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        internal static object GetDefaultValue(this Type type)
        {
            if (type == null) 
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(void))
                return null;

            var prodiver = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type), typeof(object)
                )
            ).Compile();

            return prodiver();
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