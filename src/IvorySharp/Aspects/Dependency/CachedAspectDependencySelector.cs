using System;
using IvorySharp.Caching;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Селектор зависимостей аспектов с поддержкой кеша.
    /// </summary>
    internal class CachedAspectDependencySelector : IAspectDependencySelector
    {
        private readonly Func<Type, AspectPropertyDependency[]> _cachedPropertyDependencyProvider;
        private readonly Func<Type, bool> _cachedHasDependencies;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedAspectDependencySelector"/>.
        /// </summary>
        public CachedAspectDependencySelector(
            IAspectDependencySelector dependencySelector, 
            ICacheDelegateFactory cacheDelegateFactory)
        {
            _cachedPropertyDependencyProvider = cacheDelegateFactory.CreateDelegate<Type, AspectPropertyDependency[]>(
                dependencySelector.SelectPropertyDependencies);
            
            _cachedHasDependencies = cacheDelegateFactory.CreateDelegate<Type, bool>(
                dependencySelector.HasDependencies);
        }

        /// <inheritdoc />
        public AspectPropertyDependency[] SelectPropertyDependencies(Type aspectType)
        {
            return _cachedPropertyDependencyProvider(aspectType);
        }

        /// <inheritdoc />
        public bool HasDependencies(Type aspectType)
        {
            return _cachedHasDependencies(aspectType);
        }
    }
}