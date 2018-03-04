using System;
using IvorySharp.Exceptions;
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
            try
            {
                return _container.GetInstance<TService>();
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
            if (key != null)
            {
                throw new NotSupportedException(
                    $"Получение именованных зависимостей не поддерживается SimpleInjector'om. {nameof(key)}:{key}");
            }

            try
            {
                return _container.GetInstance<TService>();
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
                return _container.GetInstance(serviceType);
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
            if (key != null)
            {
                throw new NotSupportedException(
                    $"Получение именованных зависимостей не поддерживается SimpleInjector'om. {nameof(key)}:{key}");
            }

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
    }
}