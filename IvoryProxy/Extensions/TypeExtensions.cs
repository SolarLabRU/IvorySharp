using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IvoryProxy.Extensions
{
    internal static class TypeExtensions
    {
        public static bool HasAttribute<TAttibute>(this Type type, bool inherit) where TAttibute : Attribute
        {
            return type.GetCustomAttribute<TAttibute>(inherit) != null;
        }

        public static bool HasAttribte<TAttibute>(this MethodBase method, bool inherit) where TAttibute : Attribute
        {
            return method.GetCustomAttribute<TAttibute>(inherit) != null;
        }
    }
}
