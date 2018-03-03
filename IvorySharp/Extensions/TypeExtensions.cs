using System;
using System.Reflection;
using IvorySharp.Aspects.Configuration;

namespace IvorySharp.Extensions
{
    internal static class TypeExtensions
    {
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool IsWeavable(this Type type, IWeavingAspectsConfiguration configurations)
        {
            if (configurations.ExplicitWeaingAttributeType == null)
            {
                return true;
            }

            return type.GetCustomAttribute(configurations.ExplicitWeaingAttributeType) != null;
        }
    }
}