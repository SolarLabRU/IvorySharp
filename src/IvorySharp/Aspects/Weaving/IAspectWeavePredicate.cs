using System;
using System.ComponentModel;
using IvorySharp.Core;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат для определения возможности применения аспектов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectWeavePredicate : IComponent
    {
        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="declaredType">Тип, в котором объявлен вызываемый метод.</param>
        /// <param name="targetType">Исходный тип объекта, метод которого был вызван.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable(Type declaredType, Type targetType);

        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="signature">Сигнатура вызова метода.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable(IInvocationSignature signature);
    }
}