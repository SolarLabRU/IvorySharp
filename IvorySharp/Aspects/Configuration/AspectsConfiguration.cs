using System;
using IvorySharp.Aspects.Integration;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель конфигурации аспектов.
    /// </summary>
    public class AspectsConfiguration
    {
        private readonly AspectsContainer _aspectsContainer;
        
        /// <summary>
        /// Набор настроек для аспектов.
        /// </summary>
        internal WeavingAspectsConfiguration Configuration { get; }

        internal AspectsConfiguration(AspectsContainer aspectsContainer)
        {
            _aspectsContainer = aspectsContainer;
            Configuration = new WeavingAspectsConfiguration();
        }

        public void UseExplicitWeavingAttribute<TAttribute>() where TAttribute : Attribute
        {
            Configuration.ExplicitWeaingAttributeType = typeof(TAttribute);
        }
    }
}