using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент для предварительной инициализации аспектов.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    internal interface IAspectPreInitializer<out TAspect>: IComponent
        where TAspect : OrderableMethodAspect
    {
        /// <summary>
        /// Подготавливает аспекты для инициализации.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Массив аспектов.</returns>
        TAspect[] PrepareAspects(IInvocationContext context);
    }
}