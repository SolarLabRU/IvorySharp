using System;
using System.Collections.Generic;
using System.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Describes the signature of method invocation.
    /// </summary>
    internal sealed class InvocationSignature : IInvocationSignature, IEquatable<InvocationSignature>
    {
        /// <inheritdoc />
        public MethodInfo Method { get; }

        /// <inheritdoc />
        public MethodInfo TargetMethod { get; }

        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <inheritdoc />
        public Type TargetType { get; }

        /// <inheritdoc />
        public InvocationType InvocationType { get; }
        
        /// <summary>
        /// Creates a new instance of <see cref="InvocationSignature"/>.
        /// </summary>
        public InvocationSignature(
            MethodInfo method,
            MethodInfo targetMethod, 
            Type declaringType,
            Type targetType,
            InvocationType invocationType)
        {
            Method = method;
            TargetMethod = targetMethod;
            DeclaringType = declaringType;
            TargetType = targetType;
            InvocationType = invocationType;
        }
        
        /// <inheritdoc />
        public bool Equals(InvocationSignature other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Method.Equals(other.Method) && 
                   TargetMethod.Equals(other.TargetMethod) && 
                   DeclaringType == other.DeclaringType &&
                   TargetType == other.TargetType &&
                   InvocationType == other.InvocationType;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is InvocationSignature signature && Equals(signature);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Method.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetMethod.GetHashCode();
                hashCode = (hashCode * 397) ^ DeclaringType.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetType.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) InvocationType;
                return hashCode;
            }
        }
        
        /// <summary>
        /// Compares two invocation signatures based on underlying method <see cref="IInvocationSignature.Method"/>.
        /// </summary>
        public sealed class InvocationSignatureMethodEqualityComparer : EqualityComparer<IInvocationSignature>
        {
            /// <summary>
            /// Initialized instance of <see cref="InvocationSignatureMethodEqualityComparer"/>.
            /// </summary>
            public static readonly InvocationSignatureMethodEqualityComparer Instance 
                = new InvocationSignatureMethodEqualityComparer();
            
            /// <inheritdoc />
            public override bool Equals(IInvocationSignature x, IInvocationSignature y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Method.Equals(y.Method) && x.TargetMethod.Equals(y.TargetMethod);
            }

            /// <inheritdoc />
            public override int GetHashCode(IInvocationSignature obj)
            {
                if (obj == null)
                    return 0;
                
                unchecked
                {
                    return (obj.Method.GetHashCode() * 397) ^ obj.TargetMethod.GetHashCode();
                }
            }
        }

    }
}