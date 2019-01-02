using System;
using System.ComponentModel;
using System.Dynamic;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Контекст вызова метода.
    /// </summary>
    [PublicAPI]
    public interface IInvocationContext : IInvocationSignature
    {
        /// <summary>
        /// Уникальный идентификатор контекста выполнения метода.
        /// </summary>
        Guid ContextId { get; }
        
        /// <summary>
        /// Параметры вызова метода.
        /// </summary>
        [NotNull] InvocationArguments Arguments { get; }
        
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