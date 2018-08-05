using System;
using IvorySharp.Aspects.Integration;

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
            var dependencyProvider = _container.GetDependencyProvider();
            var defaultComponents = new DefaultComponentsStore(dependencyProvider);

            _componentsStore.DependencyProvider = dependencyProvider;
            
            if (_componentsStore.AspectSelector == null)
                _componentsStore.AspectSelector = defaultComponents.AspectSelector;

            if (_componentsStore.AspectWeavePredicate == null)
                _componentsStore.AspectWeavePredicate = defaultComponents.AspectWeavePredicate;

            if (_componentsStore.AspectDeclarationCollector == null)
                _componentsStore.AspectDeclarationCollector = defaultComponents.AspectDeclarationCollector;
                
            if (_componentsStore.AspectPipelineExecutor == null)
                _componentsStore.AspectPipelineExecutor = defaultComponents.AspectPipelineExecutor;

            if (_componentsStore.AspectDependencyInjector == null)
                _componentsStore.AspectDependencyInjector = defaultComponents.AspectDependencyInjector;

            if (_componentsStore.AspectOrderStrategy == null)
                _componentsStore.AspectOrderStrategy = defaultComponents.AspectOrderStrategy;

            if (_componentsStore.AspectFactory == null)
                _componentsStore.AspectFactory = defaultComponents.AspectFactory;

            configurator(_aspectsConfiguration);
            
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