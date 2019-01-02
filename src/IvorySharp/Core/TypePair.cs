using System;

namespace IvorySharp.Core
{
    /// <summary>
    /// Represents container that holds the pair of related types.
    /// </summary>
    internal readonly struct TypePair : IEquatable<TypePair>
    {
        /// <summary>
        /// Component declaring type (interface).
        /// </summary>
        public readonly Type DeclaringType;
        
        /// <summary>
        /// Component implementation type.
        /// </summary>
        public readonly Type ImplementationType;

        /// <summary>
        /// Creates a new instance of <see cref="TypePair"/>.
        /// </summary>
        public TypePair(Type declaringType, Type implementationType)
        {
            DeclaringType = declaringType;
            ImplementationType = implementationType;
        }
        
        /// <inheritdoc />
        public bool Equals(TypePair other)
        {
            return DeclaringType == other.DeclaringType && ImplementationType == other.ImplementationType;
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
                return (DeclaringType.GetHashCode() * 397) ^ ImplementationType.GetHashCode();
            }
        }
    }
}