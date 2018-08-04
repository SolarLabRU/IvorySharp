using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Configuration;
using IvorySharp.CastleWindsor.Aspects.Integration;
using IvorySharp.SimpleInjector.Aspects.Integration;
using IvorySharp.Tests.Services;
using SimpleInjector;

namespace IvorySharp.Tests.Helpers
{
    public class WeavedServiceProvider<TService, TImplementation>
        where TService : class
        where TImplementation : class, TService, new()
    {
        private WindsorContainer _windsorContainer;
        private Container _simpleInjectorContainer;
        private AspectWeaver _aspectWeaver;

        public WeavedServiceProvider(IComponentsStore components)
        {
            _windsorContainer = new WindsorContainer();
            _simpleInjectorContainer = new Container();

            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(_windsorContainer))
                .Initialize();
            
            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(_simpleInjectorContainer))
                .Initialize();

            _windsorContainer.Register(
                Component
                    .For<TService>()
                    .ImplementedBy<TImplementation>()
            );
            
            _windsorContainer.Register(
                Component
                    .For<IMultiplyService>()
                    .ImplementedBy<MultiplyService>()
            );
            
            _windsorContainer.Register(
                Component
                    .For<IDependencyService>()
                    .ImplementedBy<DependencyService>()
            );
            
            _simpleInjectorContainer.Register<TService, TImplementation>();
            _simpleInjectorContainer.Register<IMultiplyService, MultiplyService>();
            _simpleInjectorContainer.Register<IDependencyService, DependencyService>();

            _aspectWeaver = new AspectWeaver(components.AspectWeavePredicate, components.AspectPipelineExecutor, components.AspectInitializer);
        }

        public TService GetService(ServiceStoreType storeType)
        {
            switch (storeType)
            {
                case ServiceStoreType.TransientWeaving:
                    return (TService) _aspectWeaver.Weave(new TImplementation(), typeof(TService), typeof(TImplementation));
                case ServiceStoreType.CastleWindsor:
                    return _windsorContainer.Resolve<TService>();
                case ServiceStoreType.SimpleInjector:
                    return _simpleInjectorContainer.GetInstance<TService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(storeType), storeType, null);
            }
        }
    }
}