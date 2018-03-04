using System;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый атрибут аспекта.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public abstract class AspectAttribute : Attribute
    {
    }
}