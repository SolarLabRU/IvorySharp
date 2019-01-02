using System.Collections.Generic;
using System.Reflection;

namespace IvorySharp.Comparers
{
    /// <summary>
    /// Equality comparer for <see cref="MethodInfo"/>.
    /// </summary>
    internal sealed class MethodEqualityComparer : EqualityComparer<MethodInfo>
    {
        /// <summary>
        /// Initialized instance of <see cref="MethodEqualityComparer"/>.
        /// </summary>
        public static readonly MethodEqualityComparer Instance = new MethodEqualityComparer();

        private MethodEqualityComparer() { }

        /// <inheritdoc />
        public override bool Equals(MethodInfo left, MethodInfo right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            if (left.DeclaringType != right.DeclaringType)
                return false;

            if (left.ReturnType != right.ReturnType)
                return false;

            if (left.CallingConvention != right.CallingConvention)
                return false;

            if (left.IsStatic != right.IsStatic)
                return false;

            if (left.Name != right.Name)
                return false;

            var leftGenericParameters = left.GetGenericArguments();
            var rightGenericParameters = right.GetGenericArguments();
            if (leftGenericParameters.Length != rightGenericParameters.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < leftGenericParameters.Length; i++)
            {
                if (leftGenericParameters[i] != rightGenericParameters[i])
                    return false;
            }

            var leftParameters = left.GetParameters();
            var rightParameters = right.GetParameters();
            if (leftParameters.Length != rightParameters.Length)
                return false;

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < leftParameters.Length; i++)
            {
                if (leftParameters[i].ParameterType != rightParameters[i].ParameterType)
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode(MethodInfo obj)
        {
            if (obj == null || obj.DeclaringType == null)
                return 0;

            var hashCode = obj.DeclaringType.GetHashCode();
            hashCode ^= obj.Name.GetHashCode();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var parameter in obj.GetParameters())
            {
                hashCode ^= parameter.ParameterType.GetHashCode();
            }

            return hashCode;
        }
    }
}