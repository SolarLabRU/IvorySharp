using IvorySharp.Core;

namespace IvorySharp.Aspects.Components.Selection
{
    /// <summary>
    /// Компонент, агрегирующий получение аспектов вызова.
    /// </summary>
    public interface IAspectDeclarationCollector
    {
        /// <summary>
        /// Собирает аспекты, которые необходимо внедрить в вызов.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Массив деклараций аспектов для применения.</returns>
        MethodAspectDeclaration<TAspect>[] CollectAspectDeclarations<TAspect>(InvocationContext context)
            where TAspect : MethodAspect;
    }
}
