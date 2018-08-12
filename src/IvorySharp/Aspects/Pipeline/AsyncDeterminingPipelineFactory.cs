using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Pipeline.Async;
using IvorySharp.Aspects.Pipeline.Synchronous;
using IvorySharp.Comparers;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Реализация фабрики компонентов пайплайна. Выполняет определение компонентов
    /// на основе того, является ли метод асинхронным.
    /// </summary>
    internal class AsyncDeterminingPipelineFactory : IInvocationPipelineFactory
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="AsyncDeterminingPipelineFactory"/>.
        /// </summary>
        internal static readonly AsyncDeterminingPipelineFactory Instance
            = new AsyncDeterminingPipelineFactory();
        
        private readonly ConcurrentDictionary<CacheKey, bool> _isAsyncCache;

        private AsyncDeterminingPipelineFactory()
        {
            _isAsyncCache = new ConcurrentDictionary<CacheKey, bool>();
        }

        /// <inheritdoc />
        public IInvocationPipeline CreatePipeline(
            IInvocation invocation,
            IReadOnlyCollection<MethodBoundaryAspect> boundaryAspects, 
            MethodInterceptionAspect interceptionAspect)
        {
            if (IsAsync(invocation))
                return new AsyncAspectInvocationPipeline(invocation, boundaryAspects, interceptionAspect);
            
            return new SyncAspectInvocationPipeline(invocation, boundaryAspects, interceptionAspect);
        }

        /// <inheritdoc />
        public IInvocationPipelineExecutor CreateExecutor(IInvocation invocation)
        {
            if (IsAsync(invocation))
                return AsyncAspectInvocationPipelineExecutor.Instance;
            
            return SyncAspectInvocationPipelineExecutor.Instance;
        }

        /// <summary>
        /// Выполняет проверку того, что вызов <paramref name="invocation"/> является асинхронным.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Признак того, что метод является асинхронным.</returns>
        internal bool IsAsync(IInvocation invocation)
        {
            var key = new CacheKey(invocation.Context.TargetType, invocation.Context.Method);
            return _isAsyncCache.GetOrAdd(key, k =>
            {
                var targetMethod = ReflectedMethod.GetMethodMap(k.TargetType, k.DeclaringMethod);
                
                if (targetMethod == null)
                    throw new InvalidOperationException(
                        $"Не удалось найти (или выбрать корректную реализацию) метода '{k.DeclaringMethod.Name}' " +
                        $"в типе '{invocation.Context.TargetType}'. " +
                        $"Проверьте наличие этого метода в интерфейсе '{invocation.Context.DeclaringType.Name}'.");
                
                return targetMethod.IsAsync();
            });
        }
        
        /// <summary>
        /// Ключ кеша для хранения признака <see cref="AsyncDeterminingPipelineFactory.IsAsync"/>.
        /// </summary>
        private class CacheKey
        {
            public readonly Type TargetType;
            public readonly MethodInfo DeclaringMethod;

            public CacheKey(Type targetType, MethodInfo declaringMethod)
            {
                TargetType = targetType;
                DeclaringMethod = declaringMethod;
            }

            private bool Equals(CacheKey other)
            {
                return TargetType == other.TargetType &&
                       MethodEqualityComparer.Instance.Equals(DeclaringMethod, other.DeclaringMethod);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((CacheKey) obj);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                unchecked
                {
                    return (TargetType.GetHashCode() * 397) ^ DeclaringMethod.GetHashCode();
                }
            }
        }
    }
}