using System;
using System.ComponentModel;

namespace IvorySharp.Components
{
    /// <summary>
    /// Lazy components store (based of <see cref="Lazy{T}"/>).
    /// </summary>
    /// <typeparam name="TComponent">Component type.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class LazyComponentHolder<TComponent> : ComponentHolderBase<TComponent>
        where TComponent : IComponent
    {
        private Lazy<TComponent> _instanceProvider;

        /// <summary>
        /// Creates a new instance of  <see cref="LazyComponentHolder{TComponent}"/>.
        /// </summary>
        /// <param name="provider">Component factory.</param>
        public LazyComponentHolder(Func<TComponent> provider)
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