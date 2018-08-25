using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Контекст вызова метода.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInvocationContext : IInvocationSignature
    {
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        InvocationArguments Arguments { get; }
        
        /// <summary>
        /// Прокси целевого объекта.
        /// </summary>
        object Proxy { get; }
        
        /// <summary>
        /// Экземпляр целевого объекта.
        /// </summary>
        object Target { get; }
    }
}