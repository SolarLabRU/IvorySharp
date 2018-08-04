using System;
using System.Collections.Generic;

namespace IvorySharp.Helpers
{
    /// <summary>
    /// Хранит пару типов.
    /// </summary>
    internal class TypePair
    {
        public TypePair(Type declaringType, Type targetType)
        {
            DeclaringType = declaringType;
            TargetType = targetType;
        }

        /// <summary>
        /// Объявленный тип.
        /// </summary>
        public Type DeclaringType { get; }
        
        /// <summary>
        /// Целевой тип.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Сравнивает пары на равенство.
        /// </summary>
        internal class EqualityComparer : EqualityComparer<TypePair>
        {      
            /// <summary>
            /// Инициализированный экземпляр <see cref="EqualityComparer"/>.
            /// </summary>
            public static readonly EqualityComparer Instance = new EqualityComparer();

            private EqualityComparer() {  }

            /// <inheritdoc />
            public override bool Equals(TypePair x, TypePair y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.DeclaringType == y.DeclaringType && x.TargetType == y.TargetType;
            }

            /// <inheritdoc />
            public override int GetHashCode(TypePair obj)
            {
                var hash = 17;

                if (obj == null)
                    return hash;

                if (obj.DeclaringType != null)
                    hash = hash * 23 + obj.DeclaringType.GetHashCode();

                if (obj.TargetType != null)
                    hash = hash * 23 + obj.TargetType.GetHashCode();

                return hash;
            }
        }
    }
}
