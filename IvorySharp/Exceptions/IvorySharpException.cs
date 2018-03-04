using System;
using System.Runtime.Serialization;

namespace IvorySharp.Exceptions
{
    /// <summary>
    /// Исключение, возникающее при работе с библиотекой.
    /// </summary>
    [Serializable]
    public class IvorySharpException : Exception
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="IvorySharpException"/>.
        /// </summary>
        public IvorySharpException()
        {
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvorySharpException"/>.
        /// </summary>
        public IvorySharpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvorySharpException"/>.
        /// </summary>
        public IvorySharpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvorySharpException"/>.
        /// </summary>
        internal IvorySharpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}