using System.ComponentModel;

namespace IvorySharp.Components
{
    /// <summary>
    /// Describes a component holder.
    /// </summary>
    /// <typeparam name="TComponent">Component type.</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IComponentHolder<TComponent> where TComponent : IComponent
    {
        /// <summary>
        /// Returns component instance.
        /// </summary>
        /// <returns>Component.</returns>
        TComponent Get();

        /// <summary>
        /// Replaces component inside with new instance.
        /// </summary>
        /// <param name="component">New component.</param>
        void Replace(TComponent component);

        /// <summary>
        /// Freezes the holder (prohibits the component replacement).
        /// </summary>
        void Freeze();
    }
}