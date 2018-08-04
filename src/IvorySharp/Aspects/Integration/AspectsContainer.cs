using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Configuration;

namespace IvorySharp.Aspects.Integration
{
    /// <summary>
    /// Базовый класс контейнера аспектов.
    /// </summary>
    public abstract class AspectsContainer
    {
        /// <summary>
        /// Выполняет привязку аспектов к зарегистрированным в контейнере компонентам.
        /// </summary>
        /// <param name="settings">Конфигурация.</param>
        public abstract void BindAspects(IComponentsStore settings);

        /// <summary>
        /// Возвращает сервис провайдер.
        /// </summary>
        /// <returns>Экземпляр провайдера.</returns>
        public abstract IServiceProvider GetServiceProvider();
    }
}