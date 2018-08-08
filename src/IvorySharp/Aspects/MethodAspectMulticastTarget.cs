using JetBrains.Annotations;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Перечень возможных мест для закрепления аспекта.
    /// </summary>
    [PublicAPI]
    public enum MethodAspectMulticastTarget
    {
        /// <summary>
        /// Значение по умолчанию.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Аспект задан на уровне типа.
        /// </summary>
        Type = 1,

        /// <summary>
        /// Аспект задан на уровне метода.
        /// </summary>
        Method = 2
    }
}