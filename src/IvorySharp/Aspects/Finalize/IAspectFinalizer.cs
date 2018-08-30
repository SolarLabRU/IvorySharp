using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Finalize
{
    /// <summary>
    /// Компонент, выполняющий финализацию аспекта.
    /// </summary>
    [PublicAPI]
    public interface IAspectFinalizer : IComponent
    {
        /// <summary>
        /// Выполняет финализацию аспекта.
        /// </summary>
        /// <param name="methodAspect">Аспект.</param>
        void Finalize([NotNull] MethodAspect methodAspect);

        /// <summary>
        /// Возвращает признак того, что аспект поддерживает финализацию.
        /// </summary>
        /// <param name="methodAspect">Аспект.</param>
        /// <returns>Признак того, что аспект поддерживает финализацию.</returns>
        bool IsFinalizable([NotNull] MethodAspect methodAspect);
    }
}