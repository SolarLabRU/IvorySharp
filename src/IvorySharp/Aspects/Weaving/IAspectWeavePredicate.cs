using System;
using System.Reflection;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат для определения возможности применения аспектов.
    /// </summary>
    public interface IAspectWeavePredicate
    {
        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="declaringType">Тип, в котором объявлен вызываемый метод.</param>
        /// <param name="targetType">Исходный тип объекта, метод которого был вызван.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable(Type declaringType, Type targetType);

        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="method">Вызываемый метод.</param>
        /// <param name="declaringType">Тип, в котором объявлен вызываемый метод.</param>
        /// <param name="targetType">Исходный тип объекта, метод которого был вызван.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable(MethodInfo method, Type declaringType, Type targetType);
    }
}