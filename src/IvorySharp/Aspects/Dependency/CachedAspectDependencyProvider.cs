using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Caching;
using IvorySharp.Components;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Провайдер зависимостей аспекта с кешем.
    /// </summary>
    internal class CachedAspectDependencyProvider : IAspectDependencyProvider
    {
        private Func<Type, IEnumerable<AspectPropertyDependency>> _cachedProvider;
        private IComponentProvider<IAspectDependencyProvider> _aspectDependencyProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedAspectDependencyProvider"/>.
        /// </summary>
        /// <param name="aspectDependencyProvider">Исходный провайдер завичимостей.</param>
        public CachedAspectDependencyProvider(
            IComponentProvider<IAspectDependencyProvider> aspectDependencyProvider)
        {
            _aspectDependencyProvider = aspectDependencyProvider;
        }
        
        public IEnumerable<AspectPropertyDependency> GetPropertyDependencies(Type aspectType)
        {
            if (_cachedProvider == null)
                _cachedProvider = Memoizer.CreateProducer<Type, AspectPropertyDependency[]>(
                    t => _aspectDependencyProvider.Get().GetPropertyDependencies(t).ToArray());
            
            return _cachedProvider(aspectType);
        }
    }
}