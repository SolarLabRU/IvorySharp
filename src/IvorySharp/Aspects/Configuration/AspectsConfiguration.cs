using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настройки аспектов.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public class AspectsConfiguration
    {
        /// <summary>
        /// Набор компонентов библиотеки.
        /// </summary>
        internal MutableComponentsStore ComponentsStore { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsConfiguration"/>.
        /// </summary>
        /// <param name="componentsStore">Конфигурация обвязки аспектов.</param>
        internal AspectsConfiguration(MutableComponentsStore componentsStore)
        {
            ComponentsStore = componentsStore;
        }
    }
}