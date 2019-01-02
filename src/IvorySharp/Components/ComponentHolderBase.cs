using System;
using System.ComponentModel;

namespace IvorySharp.Components
{
    /// <summary>
    /// Basic component provider.
    /// </summary>
    /// <typeparam name="TComponent">Component type.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ComponentHolderBase<TComponent> : IComponentHolder<TComponent>
        where TComponent : IComponent
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        /// <summary>
        /// Determines when provider is "frozen" (component replacement prohibited).
        /// </summary>
        public bool IsFrozen { get; private set; }

        /// <inheritdoc />
        public abstract TComponent Get();

        /// <inheritdoc />
        public void Replace(TComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            lock (Lock)
            {
                if (IsFrozen)
                    throw new InvalidOperationException("Невозможно заменить значение, т.к. провайдер заморожен");

                ReplaceInternal(component);

                IsFrozen = true;
            }
        }

        /// <summary>
        /// Replaces the component with the new one.
        /// </summary>
        /// <param name="component">New component instance.</param>
        protected abstract void ReplaceInternal(TComponent component);

        /// <inheritdoc />
        public void Freeze()
        {
            lock (Lock)
            {  
                IsFrozen = true;
            }
        }
    }
}