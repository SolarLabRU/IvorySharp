using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Integration.Microsoft.DependencyInjection.Aspects.Integration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace IvorySharp.Integration.Microsoft.DependencyInjection
{
    /// <summary>
    /// Набор расширений для <see cref="IServiceCollection"/>.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Выполняет привязку аспектов к сервисам.
        /// Важно: вызов данного метода должен распологаться после регистрации
        /// всех зависимостей.
        /// </summary>
        /// <param name="serviceCollection">Коллекция сервисов.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection UseAspectsWeaving(
            this IServiceCollection serviceCollection)
        {
            AspectsConfigurator
                .UseContainer(new MicrosoftAspectContainer(serviceCollection))
                .Initialize();

            return serviceCollection;
        }

        /// <summary>
        /// Выполняет привязку аспектов к сервисам.
        /// Важно: вызов данного метода должен распологаться после регистрации
        /// всех зависимостей.
        /// </summary>
        /// <param name="serviceCollection">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация.</param>
        /// <returns>Коллекция сервисов.</returns>
        public static IServiceCollection UseAspectsWeaving(
            this IServiceCollection serviceCollection, Action<AspectsConfiguration> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            
            AspectsConfigurator
                .UseContainer(new MicrosoftAspectContainer(serviceCollection))
                .Initialize(configuration);

            return serviceCollection;
        }
    }
}