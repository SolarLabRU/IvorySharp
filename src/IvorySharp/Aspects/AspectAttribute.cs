using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый атрибут аспекта.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), PublicAPI, AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Class)]
    public abstract class AspectAttribute : Attribute
    {
    }
}