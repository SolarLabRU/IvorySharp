using System.Collections.Generic;
using IvorySharp.Comparers;

namespace IvorySharp.Core
{
    /// <summary>
    /// Выполняет сравнение контекстов.
    /// </summary>
    internal sealed class InvocationContextComparer : IEqualityComparer<IInvocationContext>
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="InvocationContextComparer"/>.
        /// </summary>
        public static readonly InvocationContextComparer Instance = new InvocationContextComparer();
            
        /// <inheritdoc />
        public bool Equals(IInvocationContext x, IInvocationContext y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
                
            return x.GetType() == y.GetType() && 
                   x.DeclaringType == y.DeclaringType &&
                   x.TargetType == y.TargetType &&
                   MethodEqualityComparer.Instance.Equals(x.Method, y.Method);
        }

        /// <inheritdoc />
        public int GetHashCode(IInvocationContext obj)
        {
            return obj.Method.GetHashCode();
        }
    }
}