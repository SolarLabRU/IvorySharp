using System;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Method invocation context (available after method call).
    /// </summary>
    [PublicAPI]
    public interface IInvocationContext : IInvocationSignature
    {
        /// <summary>
        /// Context identifier.
        /// </summary>
        Guid ContextId { get; }
        
        /// <summary>
        /// Method invocation arguments.
        /// </summary>
        [NotNull] InvocationArguments Arguments { get; }
        
        /// <summary>
        /// Proxy instance of target service (<see cref="Target"/>). 
        /// </summary>
        object Proxy { get; }
        
        /// <summary>
        /// Transparent instance of target service.
        /// </summary>
        object Target { get; }
    }
}