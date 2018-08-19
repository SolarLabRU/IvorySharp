using System.ComponentModel;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using JetBrains.Annotations;

namespace IvorySharp.Components
{
    /// <summary>
    /// Компоненты библиотеки.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public interface IComponentsStore 
    { 
        /// <summary>
        /// Провайдер зависимостей.
        /// </summary>
        IComponentProvider<IDependencyProvider> DependencyProvider { get; }

        /// <summary>
        /// Стратегия получения аспектов.
        /// </summary>
        IComponentProvider<IAspectSelector> AspectSelector { get; }

        /// <summary>
        /// Предикат, определяющий возможность применения аспекта.
        /// </summary>
        IComponentProvider<IAspectWeavePredicate> AspectWeavePredicate { get; }

        /// <summary>
        /// Компонент, агрегирующий аспекты вызова.
        /// </summary>
        IComponentProvider<IAspectDeclarationCollector> AspectDeclarationCollector { get; }

        /// <summary>
        /// Фабрика компонентов пайлпайна.
        /// </summary>
        IComponentProvider<IInvocationPipelineFactory> PipelineFactory { get; }

        /// <summary>
        /// Компонент, выпонялющий инициализацию аспектов.
        /// </summary>
        IComponentProvider<IAspectFactory> AspectFactory { get; }

        /// <summary>
        /// Компонент, для внедрения зависимостей в аспекты.
        /// </summary>
        IComponentProvider<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <summary>
        /// Стратегия упорядочивания аспектов.
        /// </summary>
        IComponentProvider<IAspectOrderStrategy> AspectOrderStrategy { get; }
    }
}