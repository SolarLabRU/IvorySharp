using IvorySharp.Core;

namespace IvorySharp.Aspects.Components.Creation
{
    /// <summary>
    /// Компонент, выполняющий инициализацию аспектов.
    /// </summary>
    public interface IMethodAspectInitializer
    {
        /// <summary>
        /// Инициализирует аспекты типа <see cref="MethodBoundaryAspect"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Массив аспектов.</returns>
        MethodBoundaryAspect[] InitializeBoundaryAspects(InvocationContext context);

        /// <summary>
        /// Инициализирует аспект типа <see cref="MethodInterceptionAspect"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Инициализированный аспект.</returns>
        MethodInterceptionAspect InitializeInterceptionAspect(InvocationContext context);
    }
}