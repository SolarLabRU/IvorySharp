using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects;

namespace IvorySharp.Extensions.ClassAspectSelection.Extensions
{
    internal static class TypeExtensions
    {
        private static readonly Type[] NotInterceptableTypes =
            typeof(MethodAspect).Assembly.GetTypes().ToArray();
        
        public static IEnumerable<Type> GetInterceptableBaseTypes(this Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (NotInterceptableTypes.Contains(baseType))
                    yield break;
                
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }
    }
}