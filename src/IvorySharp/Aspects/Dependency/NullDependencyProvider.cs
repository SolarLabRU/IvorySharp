using System;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Провайдер зависимостей, всегда возвращающий null.
    /// </summary>
    internal sealed class NullDependencyProvider : IDependencyProvider
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="NullDependencyProvider"/>.
        /// </summary>
        public static readonly NullDependencyProvider Instance = new NullDependencyProvider();
        
        private NullDependencyProvider() { }
        
        /// <inheritdoc />
        public TService GetService<TService>() where TService : class
        {
            return null;
        }

        /// <inheritdoc />
        public TService GetTransparentService<TService>() where TService : class
        {
            return null;
        }

        /// <inheritdoc />
        public TService GetNamedService<TService>(string key) where TService : class
        {
            return null;
        }

        /// <inheritdoc />
        public TService GetTransparentNamedService<TService>(string key) where TService : class
        {
            return null;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return null;
        }

        /// <inheritdoc />
        public object GetTransparentService(Type serviceType)
        {
            return null;
        }

        /// <inheritdoc />
        public object GetNamedService(Type serviceType, string key)
        {
            return null;
        }

        /// <inheritdoc />
        public object GetTransparentNamedService(Type serviceType, string key)
        {
            return null;
        }
    }
}