using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.CastleWindsor.Aspects.Integration;
using IvorySharp.SimpleInjector.Aspects.Integration;
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

        public WeavedServiceProvider(TService instance, IWeavingAspectsConfiguration configuration)
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
            
            _simpleInjectorContainer.Register<TService, TImplementation>();

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