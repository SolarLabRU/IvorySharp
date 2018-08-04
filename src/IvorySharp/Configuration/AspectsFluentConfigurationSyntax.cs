using System;
using IvorySharp.Integration;

namespace IvorySharp.Configuration
{
    /// <summary>
    /// Синтаксис конфигурации аспектов.
    /// </summary>
    public class AspectsFluentConfigurationSyntax
    {
        private readonly AspectsWeavingSettings _aspectsWeavingSettings;
        private readonly AspectsConfiguration _aspectsConfiguration;
        private readonly AspectsContainer _container;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsFluentConfigurationSyntax"/>.
        /// </summary>
        /// <param name="aspectsContainer">Контейнер аспектов.</param>
        internal AspectsFluentConfigurationSyntax(AspectsContainer aspectsContainer)
        {
            _container = aspectsContainer;
            _aspectsWeavingSettings = new AspectsWeavingSettings();
            _aspectsConfiguration = new AspectsConfiguration(_aspectsWeavingSettings);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        /// <param name="configurator">Конфигуратор настроек.</param>
        public void Initialize(Action<AspectsConfiguration> configurator)
        {
            configurator(_aspectsConfiguration);
            _aspectsWeavingSettings.ServiceProvider = _container.GetServiceProvider();
            _container.BindAspects(_aspectsConfiguration.AspectsWeavingSettings);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        public void Initialize()
        {
            Initialize(_ => { });
        }
    }
}