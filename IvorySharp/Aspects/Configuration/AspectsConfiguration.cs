using System;
using IvorySharp.Aspects.Integration;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настройки аспектов.
    /// </summary>
    public class AspectsConfiguration
    {
        private readonly AspectsContainer _aspectsContainer;
        
        /// <summary>
        /// Набор настроек для аспектов.
        /// </summary>
        internal AspectsWeavingSettings WeavingSettings { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsConfiguration"/>.
        /// </summary>
        /// <param name="aspectsContainer">Контейнер аспектов.</param>
        /// <param name="weavingSettings">Конфигурация обвязки аспектов.</param>
        internal AspectsConfiguration(AspectsContainer aspectsContainer, AspectsWeavingSettings weavingSettings)
        {
            _aspectsContainer = aspectsContainer;
            WeavingSettings = weavingSettings;
        }

        /// <summary>
        /// Устанавливает явное включение поддержки аспектов для всех типов, которые помечены
        /// атрибутом <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        public void UseExplicitWeavingAttribute<TAttribute>() where TAttribute : Attribute
        {
            WeavingSettings.ExplicitWeaingAttributeType = typeof(TAttribute);
        }
    }
}