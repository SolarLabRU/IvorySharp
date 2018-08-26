using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Генерирует значение по умолчанию для заданного типа.
    /// </summary>
    [CanBeNull]
    public delegate object DefaultValueGenerator();
}