using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Сигнатура вызова метода.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
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
        /// Инициализирует экземпляр <see cref="InvocationSignature"/>.
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
    }
}