using System;
using System.ComponentModel;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат возможности применения аспекта с кешем.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CachedWeavePredicate : IAspectWeavePredicate
    {
        private readonly IComponentProvider<IAspectWeavePredicate> _predicateProvider;

        private Func<IInvocation, bool> _isWeaveableInvocationCached;
        private Func<TypePair, bool> _isWeaveableTypeCached;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedWeavePredicate"/>.
        /// </summary>
        /// <param name="predicateProvider">Исходный предикат.</param>
        public CachedWeavePredicate(IComponentProvider<IAspectWeavePredicate> predicateProvider)
        {
            _predicateProvider = predicateProvider;
        }
        
        /// <inheritdoc />
        public bool IsWeaveable(Type declaringType, Type targetType)
        {
            if (_isWeaveableTypeCached == null)
                _isWeaveableTypeCached = Memoizer.CreateProducer<TypePair, bool>(
                    tp => _predicateProvider.Get().IsWeaveable(tp.DeclaringType, tp.TargetType));

            return _isWeaveableTypeCached(new TypePair(declaringType, targetType));
        }

        /// <inheritdoc />
        public bool IsWeaveable(IInvocation invocation)
        {
            if (_isWeaveableInvocationCached == null)
                _isWeaveableInvocationCached = Memoizer.CreateProducer<IInvocation, bool>(
                    i => _predicateProvider.Get().IsWeaveable(i));

            return _isWeaveableInvocationCached(invocation);
        }
        
        /// <summary>
        /// Пара типов.
        /// </summary>
        private sealed class TypePair
        {
            public readonly Type DeclaringType;
            public readonly Type TargetType;

            public TypePair(Type declaringType, Type targetType)
            {
                DeclaringType = declaringType;
                TargetType = targetType;
            }

            private bool Equals(TypePair other)
            {
                return DeclaringType == other.DeclaringType &&
                       TargetType == other.TargetType;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((TypePair) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((DeclaringType != null ? DeclaringType.GetHashCode() : 0) * 397) ^ 
                           (TargetType != null ? TargetType.GetHashCode() : 0);
                }
            }
        }
    }
}