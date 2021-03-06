﻿using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Integration.CastleWindsor.Aspects.Integration;
using IvorySharp.Integration.Microsoft.DependencyInjection;
using IvorySharp.Integration.SimpleInjector.Aspects.Integration;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly Action<AspectsConfiguration> _configurator;

        private IServiceProvider _serviceProvider;
        private bool _isInitialized;

        public Weaved(IComponentsStore components, Action<AspectsConfiguration> configurator)
            : this(components, new Container(), new WindsorContainer(), Array.Empty<DependencyPair>())
        {
            _configurator = configurator;
        }

        public Weaved(IComponentsStore components, params DependencyPair[] dependency)
            : this(components, new Container(), new WindsorContainer(), dependency)
        {
        }

        public Weaved(IComponentsStore components, Container simpleInjectorContainer,
            WindsorContainer windsorContainer, params DependencyPair[] dependency)
        {
            _dependency = dependency ?? Array.Empty<DependencyPair>();
            _windsorContainer = windsorContainer;
            _simpleInjectorContainer = simpleInjectorContainer;
            _aspectWeaver = AspectWeaverFactory.Create(components);
        }

        private void InitializeWindsor(IEnumerable<DependencyPair> dependency)
        {
            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(_windsorContainer))
                .Initialize(s =>
                {
                    if (_configurator != null)
                        _configurator(s);
                });

            _windsorContainer.Register(
                Component
                    .For<TService>()
                    .ImplementedBy<TImplementation>());

            foreach (var pair in dependency)
            {
                _windsorContainer.Register(Component.For(pair.ServiceType).ImplementedBy(pair.ImplementationType));
            }
        }

        private void InitializeSimpleInjector(IEnumerable<DependencyPair> dependency)
        {
            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(_simpleInjectorContainer))
                .Initialize(s =>
                {
                    if (_configurator != null)
                        _configurator(s);
                });

            _simpleInjectorContainer.Register<TService, TImplementation>();

            foreach (var pair in dependency)
            {
                _simpleInjectorContainer.Register(pair.ServiceType, pair.ImplementationType);
            }
        }

        private void InitializeMicrosoftDependencyInjection(IEnumerable<DependencyPair> dependency)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<TService, TImplementation>();
            
            foreach (var pair in dependency)
            {
                serviceCollection.AddTransient(pair.ServiceType, pair.ImplementationType);
            }

            if (_configurator != null)
                serviceCollection.UseAspectsWeaving(_configurator);
            else
                serviceCollection.UseAspectsWeaving();
            
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public TService Get(FrameworkType frameworkType)
        {
            if (!_isInitialized)
            {
                InitializeSimpleInjector(_dependency);
                InitializeWindsor(_dependency);
                InitializeMicrosoftDependencyInjection(_dependency);

                _isInitialized = true;
            }

            switch (frameworkType)
            {
                case FrameworkType.Native:
                    return (TService) _aspectWeaver.Weave(new TImplementation(), typeof(TService),
                        typeof(TImplementation));
                case FrameworkType.CastleWindsor:
                    return _windsorContainer.Resolve<TService>();
                case FrameworkType.SimpleInjector:
                    return _simpleInjectorContainer.GetInstance<TService>();
                case FrameworkType.MicrosoftDependencyInjection:
                    return _serviceProvider.GetService<TService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameworkType), frameworkType, null);
            }
        }
    }
}