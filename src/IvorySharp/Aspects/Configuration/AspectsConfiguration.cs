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
        internal AspectsWeavingSettings AspectsWeavingSettings { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsConfiguration"/>.
        /// </summary>
        /// <param name="aspectsContainer">Контейнер аспектов.</param>
        /// <param name="aspectsWeavingSettings">Конфигурация обвязки аспектов.</param>
        internal AspectsConfiguration(AspectsContainer aspectsContainer, AspectsWeavingSettings aspectsWeavingSettings)
        {
            _aspectsContainer = aspectsContainer;
            AspectsWeavingSettings = aspectsWeavingSettings;
        }

        /// <summary>
        /// Устанавливает явное включение поддержки аспектов для всех типов, которые помечены
        /// атрибутом <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        public void UseExplicitWeavingAttribute<TAttribute>() where TAttribute : Attribute
        {
            AspectsWeavingSettings.ExplicitWeavingAttributeType = typeof(TAttribute);
        }
    }
}