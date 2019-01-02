using System;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Describes the method call signature (it's available before method call).
    /// </summary>
    [PublicAPI]
    public interface IInvocationSignature
    {
        /// <summary>
        /// Method which call is intercepting.
        /// It lives inside of <see cref="DeclaringType"/>.
        /// </summary>
        [NotNull] MethodInfo Method { get; }
        
        /// <summary>
        /// Method that lives inside of <see cref="TargetType"/> (related to <see cref="Method"/> in <see cref="DeclaringType"/>).
        /// </summary>
        [NotNull] MethodInfo TargetMethod { get; }
        
        /// <summary>
        /// Declaring service type (interface).
        /// </summary>
        [NotNull] Type DeclaringType { get; }

        /// <summary>
        /// Actual service type (class that implements <see cref="DeclaringType"/>).
        /// </summary>
        [NotNull] Type TargetType { get; }
                    
        /// <summary>
        /// The kind of called method.
        /// </summary>
        InvocationType InvocationType { get; }
    }
}