using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Represents a method call action.
    /// </summary>
    /// <param name="target">Target instance for method call.</param>
    /// <param name="args">Method call args.</param>
    /// <returns>Method call result (null if method returns null).</returns>
    [CanBeNull, EditorBrowsable(EditorBrowsableState.Never)]
    public delegate object MethodCall([NotNull] object target, [NotNull] object[] args);
}