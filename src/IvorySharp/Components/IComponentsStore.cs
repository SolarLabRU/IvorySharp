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
        IComponentHolder<IDependencyProvider> DependencyHolder { get; }

        /// <summary>
        /// Стратегия получения аспектов.
        /// </summary>
        IComponentHolder<IAspectSelector> AspectSelector { get; }

        /// <summary>
        /// Предикат, определяющий возможность применения аспекта.
        /// </summary>
        IComponentHolder<IAspectWeavePredicate> AspectWeavePredicate { get; }

        /// <summary>
        /// Компонент, агрегирующий аспекты вызова.
        /// </summary>
        IComponentHolder<IAspectDeclarationCollector> AspectDeclarationCollector { get; }

        /// <summary>
        /// Фабрика компонентов пайлпайна.
        /// </summary>
        IComponentHolder<IInvocationPipelineFactory> PipelineFactory { get; }

        /// <summary>
        /// Компонент, выпонялющий инициализацию аспектов.
        /// </summary>
        IComponentHolder<IAspectFactory> AspectFactory { get; }

        /// <summary>
        /// Компонент, для внедрения зависимостей в аспекты.
        /// </summary>
        IComponentHolder<IAspectDependencyInjector> AspectDependencyInjector { get; }

        /// <summary>
        /// Стратегия упорядочивания аспектов.
        /// </summary>
        IComponentHolder<IAspectOrderStrategy> AspectOrderStrategy { get; }
    }
}