using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Delegate for setting value to the property.
    /// </summary>
    /// <param name="target">Object instance to set property value.</param>
    /// <param name="value">Property value to set.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void PropertySetter([NotNull] object target, [CanBeNull] object value);
}