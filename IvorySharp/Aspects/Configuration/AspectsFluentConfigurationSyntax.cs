using System;
using IvorySharp.Aspects.Integration;

namespace IvorySharp.Aspects.Configuration
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
            _aspectsConfiguration = new AspectsConfiguration(_container, _aspectsWeavingSettings);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        /// <param name="configurator">Конфигуратор настроек.</param>
        public void Initialize(Action<AspectsConfiguration> configurator)
        {
            configurator(_aspectsConfiguration);
            _container.BindAspects(_aspectsConfiguration.WeavingSettings);
            _aspectsWeavingSettings.ServiceProvider = _container.GetServiceProvider();
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        public void Initialize()
        {
            _container.BindAspects(_aspectsConfiguration.WeavingSettings);
        }
    }
}