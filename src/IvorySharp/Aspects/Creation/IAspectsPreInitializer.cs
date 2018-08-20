using System.ComponentModel;
using IvorySharp.Core;
using JetBrains.Annotations;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент подготовки аспектов к инициализации. 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectsPreInitializer : IComponent
    {
        /// <summary>
        /// Подготавливает аспекты типа <see cref="MethodBoundaryAspect"/> для инициализации.
        /// </summary>
        /// <param name="context">Контекст вызова метода.</param>
        /// <returns>Массив не инициализированных аспектов.</returns>
        [NotNull] MethodBoundaryAspect[] PrepareBoundaryAspects([NotNull] IInvocationContext context);

        /// <summary>
        /// Подготавливает аспект типа <see cref="MethodInterceptionAspect"/> для инициализации.
        /// </summary>
        /// <param name="context">Контекст вызова метода.</param>
        /// <returns>Не инициализированный аспект типа <see cref="MethodInterceptionAspect"/>.</returns>
        [NotNull] MethodInterceptionAspect PrepareInterceptAspect([NotNull] IInvocationContext context);
    }
}