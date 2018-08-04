using System;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Integration;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Синтаксис конфигурации аспектов.
    /// </summary>
    public class AspectsFluentConfigurationSyntax
    {
        private readonly MutableComponentsStore _componentsStore;
        private readonly AspectsConfiguration _aspectsConfiguration;
        private readonly AspectsContainer _container;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectsFluentConfigurationSyntax"/>.
        /// </summary>
        /// <param name="aspectsContainer">Контейнер аспектов.</param>
        internal AspectsFluentConfigurationSyntax(AspectsContainer aspectsContainer)
        {
            _container = aspectsContainer;
            _componentsStore = new MutableComponentsStore();
            _aspectsConfiguration = new AspectsConfiguration(_componentsStore);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        /// <param name="configurator">Конфигуратор настроек.</param>
        public void Initialize(Action<AspectsConfiguration> configurator)
        {
            configurator(_aspectsConfiguration);

            if (_componentsStore.AspectSelector == null)
                _componentsStore.AspectSelector = new DefaultAspectSelector();

            if (_componentsStore.AspectWeavePredicate == null)
                _componentsStore.AspectWeavePredicate = new DeclaringTypeWeavePredicate(
                    _componentsStore.AspectSelector);

            if (_componentsStore.AspectDeclarationCollector == null)
                _componentsStore.AspectDeclarationCollector = new DeclaringTypeAspectDeclarationCollector(
                    _componentsStore.AspectSelector);

            if (_componentsStore.AspectPipelineExecutor == null)
                _componentsStore.AspectPipelineExecutor = AspectInvocationPipelineExecutor.Instance;

            _componentsStore.DependencyProvider = _container.GetDependencyProvider();

            if (_componentsStore.AspectDependencyInjector == null)
                _componentsStore.AspectDependencyInjector = new AspectDependencyInjector(
                    _componentsStore.DependencyProvider);

            if (_componentsStore.AspectOrderStrategy == null)
                _componentsStore.AspectOrderStrategy = new DefaultAspectOrderStrategy();

            if (_componentsStore.AspectFactory == null)
                _componentsStore.AspectFactory = new AspectFactory(
                    _componentsStore.AspectDeclarationCollector,
                    _componentsStore.AspectDependencyInjector,
                    _componentsStore.AspectOrderStrategy);


            _container.BindAspects(_aspectsConfiguration.ComponentsStore);
        }

        /// <summary>
        /// Инициализирует библиотеку для работы.
        /// </summary>
        public void Initialize()
        {
            Initialize(_ => { });
        }
    }
}