using System;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Атрибут запрещает применение аспектов на установленный метод или интерфейс.
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class SuppressAspectsWeavingAttribute : Attribute
    {     
    }
}