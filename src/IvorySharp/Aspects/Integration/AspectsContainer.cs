using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Integration
{
    /// <summary>
    /// Базовый класс контейнера аспектов.
    /// </summary>
    [PublicAPI]
    public abstract class AspectsContainer
    {
        /// <summary>
        /// Выполняет привязку аспектов к зарегистрированным в контейнере компонентам.
        /// </summary>
        /// <param name="settings">Конфигурация.</param>
        public abstract void BindAspects([NotNull] IComponentsStore settings);

        /// <summary>
        /// Возвращает сервис провайдер.
        /// </summary>
        /// <returns>Экземпляр провайдера.</returns>
        [NotNull] public abstract IDependencyProvider GetDependencyProvider();
    }
}