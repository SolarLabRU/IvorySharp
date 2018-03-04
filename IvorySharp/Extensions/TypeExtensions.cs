using System;
using System.Linq.Expressions;
using System.Reflection;
using IvorySharp.Aspects.Configuration;

namespace IvorySharp.Extensions
{
    internal static class TypeExtensions
    {
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
    }
}