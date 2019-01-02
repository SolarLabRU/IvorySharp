using System;
using System.ComponentModel;

namespace IvorySharp.Components
{
    /// <summary>
    /// Component store based on created component instance.
    /// Actually it just wraps component instance.
    /// </summary>
    /// <typeparam name="TComponent">Component type.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class InstanceComponentHolder<TComponent> : ComponentHolderBase<TComponent>
        where TComponent : IComponent
    {
        private TComponent _instance;

        /// <summary>
        /// Creates a new instance of <see cref="InstanceComponentHolder{TComponent}"/>.
        /// </summary>
        /// <param name="instance">Component instance.</param>
        public InstanceComponentHolder(TComponent instance)
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