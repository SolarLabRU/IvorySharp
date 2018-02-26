using System;

namespace IvoryProxy.Core.Attributes
{
    /// <summary>
    /// Запрещает возможность перехватывания вызова методов.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DisallowInterceptAttribute : BaseInterceptAttribute
    {
    }
}