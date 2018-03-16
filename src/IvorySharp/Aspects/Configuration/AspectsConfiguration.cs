using System;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настройки аспектов.
    /// </summary>
    public class AspectsConfiguration
    {
        /// <summary>
        /// Набор настроек для аспектов.
        /// </summary>
        internal AspectsWeavingSettings AspectsWeavingSettings { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsConfiguration"/>.
        /// </summary>
        /// <param name="aspectsWeavingSettings">Конфигурация обвязки аспектов.</param>
        internal AspectsConfiguration(AspectsWeavingSettings aspectsWeavingSettings)
        {
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