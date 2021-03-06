﻿using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace IvorySharp.Proxying.Accessors
{
    /// <summary>
    /// Вспомогательный класс для доступа к свойствам.
    /// </summary>
    internal sealed class PropertyAccessor
    {
        public MethodInfo InterfaceGetMethod { get; }
        public MethodInfo InterfaceSetMethod { get; }
        public MethodBuilder GetMethodBuilder { get; set; }
        public MethodBuilder SetMethodBuilder { get; set; }

        public PropertyAccessor(MethodInfo interfaceGetMethod, MethodInfo interfaceSetMethod)
        {
            InterfaceGetMethod = interfaceGetMethod;
            InterfaceSetMethod = interfaceSetMethod;
        }
    }
}