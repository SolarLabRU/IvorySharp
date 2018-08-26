using System;
using System.ComponentModel;
using System.Linq.Expressions;
using IvorySharp.Components;
using IvorySharp.Linq;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настройки аспектов.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class AspectsConfiguration
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

        /// <summary>
        /// Заменяет компонент библиотеки.
        /// </summary>
        /// <param name="componentSelector">Селектор компонента.</param>
        /// <typeparam name="TComponent">Тип компонента.</typeparam>
        /// <returns>Синтаксис замены компонента.</returns>
        public ReplaceComponentSyntax<TComponent> ReplaceComponent<TComponent>(
            Expression<Func<IComponentsStore, IComponentHolder<TComponent>>> componentSelector)
            where TComponent : IComponent
        {
            var propertyName = Expressions.GetMemberName(componentSelector);
            var property = typeof(MutableComponentsStore).GetProperty(propertyName);
            
            return new ReplaceComponentSyntax<TComponent>(ComponentsStore, property);
        }
    }
}