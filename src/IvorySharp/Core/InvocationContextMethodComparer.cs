using System.Collections.Generic;
using IvorySharp.Comparers;

namespace IvorySharp.Core
{
    /// <summary>
    /// Выполняет сравнение контекстов на основе метода.
    /// </summary>
    internal sealed class InvocationContextMethodComparer : IEqualityComparer<IInvocationContext>
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="InvocationContextMethodComparer"/>.
        /// </summary>
        public static readonly InvocationContextMethodComparer Instance = new InvocationContextMethodComparer();
            
        /// <inheritdoc />
        public bool Equals(IInvocationContext x, IInvocationContext y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
                
            return x.GetType() == y.GetType() && 
                   MethodEqualityComparer.Instance.Equals(x.Method, y.Method);
        }

        /// <inheritdoc />
        public int GetHashCode(IInvocationContext obj)
        {
            return obj.Method.GetHashCode();
        }
    }
}