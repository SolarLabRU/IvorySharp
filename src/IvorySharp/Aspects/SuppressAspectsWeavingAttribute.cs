using System;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Атрибут запрещает применение аспектов на установленный метод или интерфейс.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public sealed class SuppressAspectsWeavingAttribute : Attribute
    {     
    }
}