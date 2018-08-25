using System.Runtime.CompilerServices;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Фабрика для типа <see cref="AspectWeaver"/>.
    /// </summary>
    [PublicAPI]
    public sealed class AspectWeaverFactory
    {
        /// <summary>
        /// Создает новый экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <param name="componentsStore">Настройки.</param>
        /// <returns>Инициализированный экземпляр <see cref="AspectWeaver"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AspectWeaver Create(IComponentsStore componentsStore)
        {
            return new AspectWeaver(
                componentsStore.WeaveDataProviderFactory,
                componentsStore.AspectDependencyInjector,
                componentsStore.AspectFinalizer);
        }
    }
}