using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

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
        IAspectSelector AspectSelector { get; }

        /// <summary>
        /// Предикат, определяющий возможность применения аспекта.
        /// </summary>
        IAspectWeavePredicate AspectWeavePredicate { get; }

        /// <summary>
        /// Компонент, агрегирующий аспекты вызова.
        /// </summary>
        IAspectDeclarationCollector AspectDeclarationCollector { get; }

        /// <summary>
        /// Компонент выполнения пайплайна.
        /// </summary>
        IPipelineExecutor AspectPipelineExecutor { get; }

        /// <summary>
        /// Компонент, выпонялющий инициализацию аспектов.
        /// </summary>
        IAspectFactory AspectFactory { get; }

        /// <summary>
        /// Компонент, для внедрения зависимостей в аспекты.
        /// </summary>
        IAspectDependencyInjector AspectDependencyInjector { get; }

        /// <summary>
        /// Стратегия упорядочивания аспектов.
        /// </summary>
        IAspectOrderStrategy AspectOrderStrategy { get; }
    }
}