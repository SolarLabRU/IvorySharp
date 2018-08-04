using System;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Компоненты библиотеки.
    /// </summary>
    public interface IComponentsStore
    { 
        /// <summary>
        /// Провайдер зависимостей.
        /// </summary>
        IDependencyProvider DependencyProvider { get; }

        /// <summary>
        /// Стратегия получения аспектов.
        /// </summary>
        IMethodAspectSelectionStrategy AspectSelectionStrategy { get; }

        /// <summary>
        /// Предикат, определяющий возможность применения аспекта.
        /// </summary>
        IMethodAspectWeavePredicate AspectWeavePredicate { get; }

        /// <summary>
        /// Компонент, агрегирующий аспекты вызова.
        /// </summary>
        IMethodAspectDeclarationCollector AspectDeclarationCollector { get; }

        /// <summary>
        /// Компонент выполнения пайплайна.
        /// </summary>
        IMethodAspectPipelineExecutor AspectPipelineExecutor { get; }

        /// <summary>
        /// Компонент, выпонялющий инициализацию аспектов.
        /// </summary>
        IMethodAspectInitializer AspectInitializer { get; }

        /// <summary>
        /// Компонент, для внедрения зависимостей в аспекты.
        /// </summary>
        IMethodAspectDependencyInjector AspectDependencyInjector { get; }

        /// <summary>
        /// Стратегия упорядочивания аспектов.
        /// </summary>
        IMethodAspectOrderStrategy AspectOrderStrategy { get; }
    }
}