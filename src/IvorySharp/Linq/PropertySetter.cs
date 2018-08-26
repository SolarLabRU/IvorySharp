using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Делегат установки значения свойства.
    /// </summary>
    /// <param name="target">Экземпляр класса.</param>
    /// <param name="value">Значение свойства.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void PropertySetter([NotNull] object target, [CanBeNull] object value);
}