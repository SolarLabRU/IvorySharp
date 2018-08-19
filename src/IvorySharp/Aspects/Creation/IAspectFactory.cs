using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Фабрика аспектов.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    public interface IAspectFactory<out TAspect>  : IComponent
        where TAspect : OrderableMethodAspect
    {
        /// <summary>
        /// Создает инициализированные аспекты.
        /// </summary>
        /// <param name="context">Контекст.</param>
        /// <returns>Массив инициализированных аспектов.</returns>
        TAspect[] CreateAspects(IInvocationContext context);
    }
}