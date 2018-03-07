using System;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using IvorySharp.Exceptions;
using Microsoft.CSharp.RuntimeBinder;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.CastleWindsor.Aspects.Dependency
{
    /// <summary>
    /// Сервис провайдер.
    /// </summary>
    public class WindsorServiceProvider : IServiceProvider
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// Инициализирует экземпляр <see cref="WindsorServiceProvider"/>.
        /// </summary>
        public WindsorServiceProvider(IKernel kernel)
        {
            _kernel = kernel;
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
                return _kernel.Resolve(serviceType);
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
            return UnwrapProxy(service);
        }

        /// <inheritdoc />
        public object GetNamedService(Type serviceType, string key)
        {
            try
            {
                return key == null
                    ? _kernel.Resolve(serviceType)
                    : _kernel.Resolve(key, serviceType);
            }
            catch (Exception e)
            {
                throw new IvorySharpException(
                    $"Возникло исключение при получении сервиса '{serviceType?.FullName}': {e.Message}", e);
            }
        }

        /// <inheritdoc />
        public object GetTransparentNamedService(Type serviceType, string key)
        {
            var service = GetNamedService(serviceType, key);
            return UnwrapProxy(service);
        }

        internal static object UnwrapProxy(object proxy)
        {
            if (!ProxyUtil.IsProxy(proxy))
                return proxy;

            try
            {
                return ProxyUtil.GetUnproxiedInstance(proxy);
            }
            catch (RuntimeBinderException)
            {
                return proxy;
            }
        }
    }
}