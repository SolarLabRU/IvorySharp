using System.Collections.Generic;
using System.ComponentModel;

namespace IvorySharp.Core
{
    /// <summary>
    /// Компонент для сравнения <see cref="IInvocationContext"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InvocationContextComparer : EqualityComparer<IInvocationContext>
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="InvocationContextComparer"/>.
        /// </summary>
        public static readonly InvocationContextComparer Instance = new InvocationContextComparer();
        
        /// <inheritdoc />
        public override bool Equals(IInvocationContext x, IInvocationContext y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
                
            return x.Method.Equals(y.Method) && 
                   x.DeclaringType == y.DeclaringType && 
                   x.TargetType == y.TargetType;
        }

        /// <inheritdoc />
        public override int GetHashCode(IInvocationContext obj)
        {
            if (obj == null)
                return 0;
                
            unchecked
            {
                var hashCode = obj.Method.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DeclaringType.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.TargetType.GetHashCode();
                return hashCode;
            }
        }
    }
}