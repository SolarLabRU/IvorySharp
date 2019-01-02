using System;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace IvorySharp.Integration.Microsoft.DependencyInjection.Aspects.Integration
{
    /// <summary>
    /// Провайдер зависимостей.
    /// </summary>
    public class MicrosoftDependencyProvider : IDependencyProvider
    {
        private readonly Lazy<IServiceProvider> _serviceProviderContainer;
        private IServiceProvider ServiceProvider => _serviceProviderContainer.Value;

        internal MicrosoftDependencyProvider(IServiceCollection serviceCollection)
        {
            _serviceProviderContainer = new Lazy<IServiceProvider>(serviceCollection.BuildServiceProvider);
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
                return ServiceProvider.GetService(serviceType);
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

        private static object UnwrapProxy(object service)
        {
            try
            {
                var proxy = (AspectWeaveProxy) service;
                return proxy == null ? service : proxy.Target;
            }
            catch (Exception)
            {
                return service;
            }
        }
    }
}