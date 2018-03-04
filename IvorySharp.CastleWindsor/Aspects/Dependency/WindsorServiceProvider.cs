using System;
using Castle.MicroKernel;
using IvorySharp.Exceptions;
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
            try
            {
                return _kernel.Resolve<TService>();
            }
            catch (Exception e)
            {
                throw new IvorySharpException(
                    $"Возникло исключение при получении сервиса '{typeof(TService)}': {e.Message}", e);
            }
        }

        /// <inheritdoc />
        public TService GetNamedService<TService>(string key) where TService : class 
        {
            try
            {
                return key == null
                    ? _kernel.Resolve<TService>()
                    : _kernel.Resolve<TService>(key);
            } 
            catch (Exception e)
            {
                throw new IvorySharpException(
                    $"Возникло исключение при получении сервиса '{typeof(TService)}': {e.Message}", e);
            }
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
    }
}