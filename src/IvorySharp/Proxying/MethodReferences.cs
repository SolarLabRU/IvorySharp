using System;
using System.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Ссылкки на методы.
    /// </summary>
    internal static class MethodReferences
    {
        /// <summary>
        /// Ссылка на <see cref="IvoryProxy.Invoke"/>.
        /// </summary>
        public static readonly MethodInfo ProxyInvoke = 
            typeof(IvoryProxy).GetTypeInfo().GetDeclaredMethod(nameof(IvoryProxy.Invoke));
        
        /// <summary>
        /// Ссылка на <see cref="Action{T}.Invoke"/>.
        /// </summary>
        public static readonly MethodInfo ActionOfObjectArrayInvoke =
            typeof(Action<object[]>).GetTypeInfo().GetDeclaredMethod("Invoke");


        /// <summary>
        /// Ссылка на <see cref="Type.GetTypeFromHandle"/>.
        /// </summary>
        public static readonly MethodInfo GetTypeFromHandle =
            typeof(Type).GetRuntimeMethod(nameof(Type.GetTypeFromHandle), new[] {typeof(RuntimeTypeHandle)});

    }
}