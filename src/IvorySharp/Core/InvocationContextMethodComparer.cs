using System.Collections.Generic;
using IvorySharp.Comparers;

namespace IvorySharp.Core
{
    /// <summary>
    /// Выполняет сравнение контекстов на основе методов.
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
                
            return MethodEqualityComparer.Instance.Equals(x.Method, y.Method) &&
                   MethodEqualityComparer.Instance.Equals(x.TargetMethod, y.TargetMethod);
        }

        /// <inheritdoc />
        public int GetHashCode(IInvocationContext obj)
        {
            return obj.Method.GetHashCode();
        }
    }
}