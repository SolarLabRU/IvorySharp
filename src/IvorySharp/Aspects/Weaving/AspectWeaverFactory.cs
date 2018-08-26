using System;
using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Фабрика для типа <see cref="AspectWeaver"/>.
    /// </summary>
    [PublicAPI]
    public static class AspectWeaverFactory
    {
        /// <summary>
        /// Создает новый экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <param name="componentsStore">Настройки.</param>
        /// <returns>Инициализированный экземпляр <see cref="AspectWeaver"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull] public static AspectWeaver Create([NotNull] IComponentsStore componentsStore)
        {
            if (componentsStore == null)
                throw new ArgumentNullException(nameof(componentsStore));
            
            return new AspectWeaver(
                componentsStore.WeaveDataProviderFactory,
                componentsStore.AspectDependencyInjector,
                componentsStore.AspectFinalizer);
        }

        /// <summary>
        ///  Создает новый экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <param name="dependencyProvider">Провайдер зависимостей.</param>
        /// <returns>Инициализированный экземпляр <see cref="AspectWeaver"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull] public static AspectWeaver Create([NotNull] IDependencyProvider dependencyProvider)
        {
            if (dependencyProvider == null)
                throw new ArgumentNullException(nameof(dependencyProvider));
            
            var defaultComponents = new DefaultComponentsStore(dependencyProvider);
            return Create(defaultComponents);
        }

        /// <summary>
        ///  Создает новый экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <returns>Инициализированный экземпляр <see cref="AspectWeaver"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull] public static AspectWeaver Create()
        {
            return Create(NullDependencyProvider.Instance);
        }
    }
}