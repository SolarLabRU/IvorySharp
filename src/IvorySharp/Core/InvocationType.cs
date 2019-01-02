using System.Threading.Tasks;

namespace IvorySharp.Core
{
    /// <summary>
    /// Describes the method invocation kind.
    /// </summary>
    public enum InvocationType
    {
        /// <summary>
        /// Synchronous method invocation (default).
        /// </summary>
        Synchronous = 0,
        
        /// <summary>
        /// Async function (method returns <see cref="Task{TResult}"/>)
        /// </summary>
        AsyncFunction = 1,
        
        /// <summary>
        /// Async action (method returns <see cref="Task"/>)
        /// </summary>
        AsyncAction = 2
    }
}