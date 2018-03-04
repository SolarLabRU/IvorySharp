using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;

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
        public abstract void BindAspects(IAspectsWeavingSettings settings);

        /// <summary>
        /// Возвращает сервис провайдер.
        /// </summary>
        /// <returns>Экземпляр провайдера.</returns>
        public abstract IServiceProvider GetServiceProvider();
    }
}