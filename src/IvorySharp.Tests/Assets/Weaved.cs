using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.CastleWindsor.Aspects.Integration;
using IvorySharp.SimpleInjector.Aspects.Integration;
using SimpleInjector;

namespace IvorySharp.Tests.Assets
{
    public class Weaved<TService, TImplementation>
        where TService : class
        where TImplementation : class, TService, new()
    {
        private readonly WindsorContainer _windsorContainer;
        private readonly Container _simpleInjectorContainer;
        private readonly AspectWeaver _aspectWeaver;
        private readonly DependencyPair[] _dependency;

        private bool _isInitialized;

        public Weaved(IComponentsStore components, params DependencyPair[] dependency)
        {
            _dependency = dependency ?? Array.Empty<DependencyPair>();
            _windsorContainer = new WindsorContainer();
            _simpleInjectorContainer = new Container();
            _aspectWeaver = new AspectWeaver(
                components.AspectWeavePredicate, components.AspectPipelineExecutor, components.AspectFactory);
        }

        private void InitializeWindsor(IEnumerable<DependencyPair> dependency)
        {
            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(_windsorContainer))
                .Initialize();

            _windsorContainer.Register(
                Component
                    .For<TService>()
                    .ImplementedBy<TImplementation>());

            foreach (var pair in dependency)
            {
                _windsorContainer.Register(Component.For(pair.ServiceType).ImplementedBy(pair.ImplementationType));
            }
        }

        private void InitializeSimpleInjector(DependencyPair[] dependency)
        {
            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(_simpleInjectorContainer))
                .Initialize();

            _simpleInjectorContainer.Register<TService, TImplementation>();
            
            foreach (var pair in dependency)
            {
                _simpleInjectorContainer.Register(pair.ServiceType, pair.ImplementationType);
            }
        }

        public TService Get(FrameworkType frameworkType)
        {
            if (!_isInitialized)
            {
                InitializeSimpleInjector(_dependency);
                InitializeWindsor(_dependency);

                _isInitialized = true;
            }

            switch (frameworkType)
            {
                case FrameworkType.Native:
                    return (TService)_aspectWeaver.Weave(new TImplementation(), typeof(TService), typeof(TImplementation));
                case FrameworkType.CastleWindsor:
                    return _windsorContainer.Resolve<TService>();
                case FrameworkType.SimpleInjector:
                    return _simpleInjectorContainer.GetInstance<TService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameworkType), frameworkType, null);
            }
        }
    }
}