using System;
using System.ComponentModel;

namespace IvorySharp.Core
{
    /// <summary>
    /// Хранит пару типов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal struct TypePair : IEquatable<TypePair>
    {
        /// <summary>
        /// Объявленный тип компонента.
        /// </summary>
        public readonly Type DeclaredType;
        
        /// <summary>
        /// Фактический тип компонента.
        /// </summary>
        public readonly Type TargetType;

        /// <summary>
        /// Инициализирует экземпляр <see cref="TypePair"/>.
        /// </summary>
        public TypePair(Type declaredType, Type targetType)
        {
            DeclaredType = declaredType;
            TargetType = targetType;
        }
        
        /// <inheritdoc />
        public bool Equals(TypePair other)
        {
            return DeclaredType == other.DeclaredType && TargetType == other.TargetType;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TypePair pair && Equals(pair);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (DeclaredType.GetHashCode() * 397) ^ TargetType.GetHashCode();
            }
        }
    }
}