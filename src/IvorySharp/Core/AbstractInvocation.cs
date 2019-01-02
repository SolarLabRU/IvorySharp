using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using IvorySharp.Caching;
using IvorySharp.Extensions;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Abstract method invocation that not tied to method invocation kind.
    /// </summary>
    internal abstract class AbstractInvocation : IInvocation
    {
        /// <inheritdoc />
        public Guid ContextId { get; }

        /// <inheritdoc />
        public InvocationArguments Arguments { get; }

        /// <inheritdoc />
        public MethodInfo Method { get; }

        /// <inheritdoc />
        public MethodInfo TargetMethod => _targetMethodProvider.Value;
        private readonly Lazy<MethodInfo> _targetMethodProvider;

        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <inheritdoc />
        public Type TargetType { get; }

        /// <inheritdoc />
        public object Proxy { get; }

        /// <inheritdoc />
        public object Target { get; }
        
        /// <inheritdoc />
        public InvocationType InvocationType { get; }

        /// <inheritdoc />
        public abstract object ReturnValue { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractInvocation"/>.
        /// </summary>
        protected internal AbstractInvocation(
            InvocationArguments arguments,
            MethodInfo proxiedMethod,
            Type declaringType,
            Type targetType,
            object proxy,
            object target)
        {
            ContextId = Guid.NewGuid();
            Arguments = arguments;
            Method = proxiedMethod;
            DeclaringType = declaringType;
            TargetType = targetType;
            Proxy = proxy;
            Target = target;
            InvocationType = Method.GetInvocationType();           
            _targetMethodProvider = new Lazy<MethodInfo>(GetTargetMethod);
        }
        
        /// <inheritdoc />
        public abstract object Proceed();

        /// <summary>
        /// Returns related method inside <see cref="TargetType"/>
        /// that matched declared method <see cref="Method"/> inside of <see cref="DeclaringType"/>. 
        /// </summary>
        /// <returns>Target method.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull] protected MethodInfo GetTargetMethod()
        {
            if (TargetType != null && Method != null)
                return MethodInfoCache.Instance.GetMethodMap(TargetType, Method);
            
            return null;
        }
    }
}