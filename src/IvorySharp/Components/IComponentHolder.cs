namespace IvorySharp.Components
{
    /// <summary>
    /// Компонент для хранения компонентов.
    /// </summary>
    public interface IComponentHolder<TComponent> where TComponent : IComponent
    {
        /// <summary>
        /// Возвращает экземпляр компонента.
        /// </summary>
        /// <returns>Экземпляр компонента.</returns>
        TComponent Get();

        /// <summary>
        /// Заменяет текущую реализацию компонента на <paramref name="component"/>.
        /// </summary>
        /// <param name="component">Новый компонент.</param>
        void Replace(TComponent component);

        /// <summary>
        /// Замораживает контейнтер, запрещая замену компонентов на новые.
        /// </summary>
        void Freeze();
    }
}