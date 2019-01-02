using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Describes method invocation.
    /// </summary>
    [PublicAPI]
    public interface IInvocation : IInvocationContext
    {
        /// <summary>
        /// The return value of the method.
        /// </summary>
        [CanBeNull] object ReturnValue { get; set; }
    
        /// <summary>
        /// Proceeds the original (underlying) method call.
        /// </summary>
        /// <returns>The result of method call (null if method returns void).</returns>
        [CanBeNull] object Proceed();
    }
}