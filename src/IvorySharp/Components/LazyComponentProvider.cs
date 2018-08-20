using System;
using System.ComponentModel;

namespace IvorySharp.Components
{
    /// <summary>
    /// Провайдер компонентов на основе <see cref="Lazy{T}"/>.
    /// </summary>
    /// <typeparam name="TComponent">Тип компонента.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class LazyComponentProvider<TComponent> : ComponentProviderBase<TComponent>
        where TComponent : IComponent
    {
        private Lazy<TComponent> _instanceProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="LazyComponentProvider{TComponent}"/>.
        /// </summary>
        /// <param name="provider">Провайдер экземпляра.</param>
        public LazyComponentProvider(Func<TComponent> provider)
        {
            _instanceProvider = new Lazy<TComponent>(provider);
        }

        /// <inheritdoc />
        public override TComponent Get()
        {
            return _instanceProvider.Value;
        }

        /// <inheritdoc />
        protected override void ReplaceInternal(TComponent component)
        {
            _instanceProvider = new Lazy<TComponent>(() => component);
        }
    }
}