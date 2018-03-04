using System;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Атрибут запрещает применение аспектов на установленный метод или интерфейс.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public sealed class SuppressWeaving : Attribute
    {     
    }
}