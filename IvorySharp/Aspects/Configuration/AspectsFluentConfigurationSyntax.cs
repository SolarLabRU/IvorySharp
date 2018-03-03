using System;
using IvorySharp.Aspects.Integration;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Синтаксис конфигурации аспектов.
    /// </summary>
    public class AspectsFluentConfigurationSyntax
    {
        private readonly AspectsConfiguration _aspectsConfiguration;
        private readonly AspectsContainer _container;

        internal AspectsFluentConfigurationSyntax(AspectsContainer aspectsContainer)
        {
            _container = aspectsContainer;
            _aspectsConfiguration = new AspectsConfiguration(_container);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        /// <param name="configurer">Конфигуратор настроек.</param>
        public void Initialize(Action<AspectsConfiguration> configurer)
        {
            configurer(_aspectsConfiguration);
            _container.BindAspects(_aspectsConfiguration.Configuration);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        public void Initialize()
        {
            _container.BindAspects(_aspectsConfiguration.Configuration);
        }
    }
}