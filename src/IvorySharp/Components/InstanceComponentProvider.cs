using System;

namespace IvorySharp.Components
{
    /// <summary>
    /// Провайдер компонента на основе экземпляра.
    /// </summary>
    /// <typeparam name="TComponent">Тип компонента.</typeparam>
    public class InstanceComponentProvider<TComponent> : ComponentProviderBase<TComponent>
        where TComponent : IComponent
    {
        private TComponent _instance;

        /// <summary>
        /// Инициализирует экземпляр <see cref="InstanceComponentProvider{TComponent}"/>.
        /// </summary>
        /// <param name="instance">Экземпляр компонента.</param>
        public InstanceComponentProvider(TComponent instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            
            _instance = instance;
        }

        /// <inheritdoc />
        public override TComponent Get()
        {
            return _instance;
        }

        /// <inheritdoc />
        protected override void ReplaceInternal(TComponent component)
        {
            _instance = component;
        }
    }
}