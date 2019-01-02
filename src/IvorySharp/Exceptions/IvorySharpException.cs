using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace IvorySharp.Exceptions
{
    /// <summary>
    /// The exception that occurs when something goes wrong inside the library.
    /// </summary>
    [Serializable, PublicAPI]
    public sealed class IvorySharpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IvorySharpException"/>.
        /// </summary>
        public IvorySharpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IvorySharpException"/>.
        /// </summary>
        public IvorySharpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="IvorySharpException"/>.
        /// </summary>
        internal IvorySharpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}