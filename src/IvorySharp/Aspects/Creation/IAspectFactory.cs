using System.ComponentModel;
using IvorySharp.Core;
using IComponent = IvorySharp.Components.IComponent;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент, выполняющий создание аспектов.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IAspectFactory : IComponent
    {
        /// <summary>
        /// Создает аспекты типа <see cref="MethodBoundaryAspect"/> из модели вызова <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Массив инициализированных аспектов.</returns>
        MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationContext context);

        /// <summary>
        /// Создает аспект типа <see cref="MethodInterceptionAspect"/> из модели вызова <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Инициализированный аспект.</returns>
        MethodInterceptionAspect CreateInterceptionAspect(IInvocationContext context);
    }
}