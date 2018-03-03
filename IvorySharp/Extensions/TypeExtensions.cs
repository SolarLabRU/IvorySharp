using System;

namespace IvorySharp.Extensions
{
    internal static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}