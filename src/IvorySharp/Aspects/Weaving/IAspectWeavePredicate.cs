using System;
using IvorySharp.Core;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Предикат для определения возможности применения аспектов.
    /// </summary>
    [PublicAPI]
    public interface IAspectWeavePredicate : IComponent
    {
        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="declaredType">Тип, в котором объявлен вызываемый метод.</param>
        /// <param name="targetType">Исходный тип объекта, метод которого был вызван.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable([NotNull] Type declaredType, [NotNull] Type targetType);

        /// <summary>
        /// Возвращает признак возможности применения аспектов.
        /// </summary>
        /// <param name="signature">Сигнатура вызова метода.</param>
        /// <returns>Признак возможности применения аспектов.</returns>
        bool IsWeaveable([NotNull] IInvocationSignature signature);
    }
}