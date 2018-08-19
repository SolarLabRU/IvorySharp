using System;

namespace IvorySharp.Components
{
    /// <summary>
    /// Базовый провайдер компонентов.
    /// </summary>
    /// <typeparam name="TComponent">Тип компонента.</typeparam>
    public abstract class ComponentProviderBase<TComponent> : IComponentProvider<TComponent>
        where TComponent : IComponent
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        /// <summary>
        /// Признак того, что провайдер "заморожен".
        /// </summary>
        protected bool IsFrozen { get; private set; }

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
        /// Выполняет замену компонента.
        /// </summary>
        /// <param name="component">Новый компонент.</param>
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