namespace IvorySharp.Aspects
{
    /// <summary>
    /// Точка прикрепления аспекта.
    /// </summary>
    public enum MethodAspectJoinPointType
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