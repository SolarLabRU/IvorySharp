using System;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат для определения возможности применения аспектов.
    /// </summary>
    public interface IAspectWeavePredicate : IComponent
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
        /// <param name="invocation">Модель вызова метода.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable(IInvocation invocation);
    }
}