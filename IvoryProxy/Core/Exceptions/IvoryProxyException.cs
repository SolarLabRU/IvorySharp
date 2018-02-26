using System;
using System.Runtime.Serialization;

namespace IvoryProxy.Core.Exceptions
{
    /// <summary>
    /// Исключение, возникающее при работе с библиотекой 'IvoryProxy'.
    /// </summary>
    [Serializable]
    public class IvoryProxyException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IvoryProxyException"/>.
        /// </summary>
        internal IvoryProxyException()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IvoryProxyException"/>.
        /// </summary>
        internal IvoryProxyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IvoryProxyException"/>.
        /// </summary>
        internal IvoryProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="IvoryProxyException"/>.
        /// </summary>
        internal IvoryProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}