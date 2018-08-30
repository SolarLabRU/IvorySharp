using System;
using IvorySharp.Aspects.Integration;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Синтаксис конфигурации аспектов.
    /// </summary>
    [PublicAPI]
    public sealed class AspectsFluentConfigurationSyntax
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
        //  ReSharper disable once MemberCanBePrivate.Global
        public void Initialize([NotNull] Action<AspectsConfiguration> configurator)
        {
            var dependencyProvider = _container.GetDependencyProvider();
            var defaultComponents = new DefaultComponentsStore(dependencyProvider);

            _componentsStore.DependencyHolder = dependencyProvider.ToInstanceHolder();
            
            if (_componentsStore.AspectSelector == null)
                _componentsStore.AspectSelector = defaultComponents.AspectSelector;

            if (_componentsStore.AspectWeavePredicate == null)
                _componentsStore.AspectWeavePredicate = defaultComponents.AspectWeavePredicate;

            if (_componentsStore.AspectDeclarationCollector == null)
                _componentsStore.AspectDeclarationCollector = defaultComponents.AspectDeclarationCollector;
                
            if (_componentsStore.PipelineFactory == null)
                _componentsStore.PipelineFactory = defaultComponents.PipelineFactory;

            if (_componentsStore.AspectDependencyInjector == null)
                _componentsStore.AspectDependencyInjector = defaultComponents.AspectDependencyInjector;

            if (_componentsStore.AspectOrderStrategy == null)
                _componentsStore.AspectOrderStrategy = defaultComponents.AspectOrderStrategy;

            if (_componentsStore.AspectFactory == null)
                _componentsStore.AspectFactory = defaultComponents.AspectFactory;

            if (_componentsStore.WeaveDataProviderFactory == null)
                _componentsStore.WeaveDataProviderFactory = defaultComponents.WeaveDataProviderFactory;

            if (_componentsStore.AspectFinalizer == null)
                _componentsStore.AspectFinalizer = defaultComponents.AspectFinalizer;
            
            configurator(_aspectsConfiguration);
            
            _componentsStore.Freeze();
            
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