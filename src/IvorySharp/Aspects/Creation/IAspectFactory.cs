using System.ComponentModel;
using IvorySharp.Core;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Фабрика аспектов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectFactory : IComponent
    {
        /// <summary>
        /// Создает аспекты типа <see cref="MethodBoundaryAspect"/>.
        /// </summary>
        /// <param name="signature">Сигнатура вызова метода.</param>
        /// <returns>Массив не инициализированных аспектов.</returns>
        [NotNull] MethodBoundaryAspect[] CreateBoundaryAspects([NotNull] IInvocationSignature signature);

        /// <summary>
        /// Создает аспект типа <see cref="MethodInterceptionAspect"/>.
        /// </summary>
        /// <param name="signature">Сигнатура вызова метода.</param>
        /// <returns>Не инициализированный аспект типа <see cref="MethodInterceptionAspect"/>.</returns>
        [NotNull] MethodInterceptionAspect CreateInterceptAspect([NotNull] IInvocationSignature signature);
    }
}