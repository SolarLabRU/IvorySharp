using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Генерирует значение по умолчанию для заданного типа.
    /// </summary>
    [CanBeNull, EditorBrowsable(EditorBrowsableState.Never)]
    public delegate object DefaultValueGenerator();
}