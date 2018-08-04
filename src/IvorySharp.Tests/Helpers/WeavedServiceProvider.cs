using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Weaving;
using IvorySharp.CastleWindsor.Aspects.Integration;
using IvorySharp.Configuration;
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

        public WeavedServiceProvider(IAspectsWeavingSettings configuration)
        {
            _windsorContainer = new WindsorContainer();
            _simpleInjectorContainer = new Container();

            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(_windsorContainer))
                .Initialize(c =>
                {
                    if (configuration.ExplicitWeavingAttributeType != null)
                        c.UseExplicitWeavingAttribute<EnableWeavingAttribute>();
                });
            
            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(_simpleInjectorContainer))
                .Initialize(c =>
                {
                    if (configuration.ExplicitWeavingAttributeType != null)
                        c.UseExplicitWeavingAttribute<EnableWeavingAttribute>();
                });

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

            _aspectWeaver = new AspectWeaver(configuration);
        }

        public TService GetService(WeavedServiceStoreType storeType)
        {
            switch (storeType)
            {
                case WeavedServiceStoreType.TransientWeaving:
                    return (TService) _aspectWeaver.Weave(new TImplementation(), typeof(TService));
                case WeavedServiceStoreType.CastleWindsor:
                    return _windsorContainer.Resolve<TService>();
                case WeavedServiceStoreType.SimpleInjector:
                    return _simpleInjectorContainer.GetInstance<TService>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(storeType), storeType, null);
            }
        }
    }
}