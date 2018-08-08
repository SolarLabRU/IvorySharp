using System;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый атрибут аспекта.
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public abstract class AspectAttribute : Attribute
    {
    }
}