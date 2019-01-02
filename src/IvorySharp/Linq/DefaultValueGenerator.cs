using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Generates default value for specific type.
    /// </summary>
    [CanBeNull, EditorBrowsable(EditorBrowsableState.Never)]
    public delegate object DefaultValueGenerator();
}