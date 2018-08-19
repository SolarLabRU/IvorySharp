using System;
using System.ComponentModel;
using System.Reflection;
using IvorySharp.Components;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Синтаксис замены компонента.
    /// </summary>
    /// <typeparam name="TComponent">Тип компонента.</typeparam>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ReplaceComponentSyntax<TComponent> where TComponent : IComponent
    {
        private readonly MutableComponentsStore _componentsStore;
        private readonly PropertyInfo _componentProp;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ReplaceComponentSyntax{TComponent}"/>.
        /// </summary>
        internal ReplaceComponentSyntax(
            IComponentsStore componentsStore, 
            PropertyInfo componentProp)
        {
            _componentProp = componentProp;
            _componentsStore = (MutableComponentsStore)componentsStore;
        }

        /// <summary>
        /// Устанавливает для использования компонент <paramref name="component"/>.
        /// </summary>
        /// <param name="component">Компонент.</param>
        public void Use(TComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            
            var provider = (IComponentProvider<TComponent>) _componentProp.GetValue(_componentsStore);
            provider.Replace(component);
        }

        /// <summary>
        /// Устанавливает для использования компонент, сгенерированный фабрикой <paramref name="componentFactory"/>.
        /// </summary>
        /// <param name="componentFactory">Фабрика компонента.</param>
        public void Use(Func<IComponentsStore, TComponent> componentFactory)
        {
            if (componentFactory == null)
                throw new ArgumentNullException(nameof(componentFactory));

            var component = componentFactory(_componentsStore);
            if (component == null)
                throw new ArgumentException($"{nameof(componentFactory)} result null");

            var provider = (IComponentProvider<TComponent>) _componentProp.GetValue(_componentsStore);
            provider.Replace(component);
        }
    }
}