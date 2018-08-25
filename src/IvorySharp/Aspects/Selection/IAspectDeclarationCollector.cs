using System.Collections.Generic;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент, агрегирующий получение аспектов вызова.
    /// </summary>
    public interface IAspectDeclarationCollector : IComponent
    {
        /// <summary>
        /// Собирает аспекты, которые необходимо внедрить в вызов.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="signature">Сигнатура вызова.</param>
        /// <returns>Перечень деклараций аспектов для применения.</returns>
        IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(
            IInvocationSignature signature) where TAspect : MethodAspect;
    }
}
