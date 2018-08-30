using System.Collections.Generic;
using IvorySharp.Components;
using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент, агрегирующий получение аспектов вызова.
    /// </summary>
    [PublicAPI]
    public interface IAspectDeclarationCollector : IComponent
    {
        /// <summary>
        /// Собирает аспекты, которые необходимо внедрить в вызов.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="signature">Сигнатура вызова.</param>
        /// <returns>Перечень деклараций аспектов для применения.</returns>
        [NotNull] IEnumerable<MethodAspectDeclaration<TAspect>> CollectAspectDeclarations<TAspect>(
             [NotNull] IInvocationSignature signature) where TAspect : MethodAspect;
    }
}
