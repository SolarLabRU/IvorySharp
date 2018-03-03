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

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsConfiguration"/>.
        /// </summary>
        /// <param name="aspectsContainer">Контейнер аспектов.</param>
        internal AspectsConfiguration(AspectsContainer aspectsContainer)
        {
            _aspectsContainer = aspectsContainer;
            Configuration = new WeavingAspectsConfiguration();
        }

        /// <summary>
        /// Устанавливает явное включение поддержки аспектов для всех типов, которые помечены
        /// атрибутом <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        public void UseExplicitWeavingAttribute<TAttribute>() where TAttribute : Attribute
        {
            Configuration.ExplicitWeaingAttributeType = typeof(TAttribute);
        }
    }
}