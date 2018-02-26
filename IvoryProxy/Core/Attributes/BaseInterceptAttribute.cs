using System;

namespace IvoryProxy.Core.Attributes
{
    /// <summary>
    /// Базовый атрибут для обработки перехвата вызова методов.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class BaseInterceptAttribute : Attribute
    {
    }
}