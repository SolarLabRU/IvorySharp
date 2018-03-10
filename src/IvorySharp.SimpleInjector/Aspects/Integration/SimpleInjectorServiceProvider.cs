using System;
using IvorySharp.Exceptions;
using IvorySharp.Proxying;
using SimpleInjector;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.SimpleInjector.Aspects.Integration
{
    /// <summary>
    /// Провайдер сервисов.
    /// </summary>
    public class SimpleInjectorServiceProvider : IServiceProvider
    {
        private readonly Container _container;

        /// <summary>
        /// Инициализирует экземпляр <see cref="SimpleInjectorServiceProvider"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public SimpleInjectorServiceProvider(Container container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public TService GetService<TService>() where TService : class
        {
            return (TService) GetService(typeof(TService));
        }

        /// <inheritdoc />
        public TService GetTransparentService<TService>() where TService : class
        {
            return (TService) GetTransparentService(typeof(TService));
        }

        /// <inheritdoc />
        public TService GetNamedService<TService>(string key) where TService : class
        {
            return (TService) GetNamedService(typeof(TService), key);
        }

        /// <inheritdoc />
        public TService GetTransparentNamedService<TService>(string key) where TService : class
        {
            return (TService) GetTransparentNamedService(typeof(TService), key);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            try
            {
                return _container.GetInstance(serviceType);
            }
            catch (Exception e)
            {
                throw new IvorySharpException(
                    $"Возникло исключение при получении сервиса '{serviceType?.FullName}': {e.Message}", e);
            }
        }

        /// <inheritdoc />
        public object GetTransparentService(Type serviceType)
        {
            var service = GetService(serviceType);
            
            return serviceType.IsInterface 
                ? UnwrapProxy(service) 
                : service;
        }

        /// <inheritdoc />
        public object GetNamedService(Type serviceType, string key)
        {
            if (key != null)
            {
                throw new NotSupportedException(
                    $"Получение именованных зависимостей не поддерживается SimpleInjector'om. {nameof(key)}:{key}");
            }

            return GetService(serviceType);
        }

        /// <inheritdoc />
        public object GetTransparentNamedService(Type serviceType, string key)
        {
            if (key != null)
            {
                throw new NotSupportedException(
                    $"Получение именованных зависимостей не поддерживается SimpleInjector'om. {nameof(key)}:{key}");
            }

            return GetTransparentService(serviceType);
        }

        internal static object UnwrapProxy(object service)
        {
            try
            {
                var proxy = (InterceptDispatchProxy) service;
                return proxy.Instance;
            }
            catch (Exception)
            {
                return service;
            }
        }
    }
}