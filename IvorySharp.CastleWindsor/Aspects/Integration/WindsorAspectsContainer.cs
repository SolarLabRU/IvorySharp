using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Integration;
using IvorySharp.CastleWindsor.Aspects.Weaving;

namespace IvorySharp.CastleWindsor.Aspects.Integration
{
    /// <summary>
    /// Контейнер аспектов.
    /// </summary>
    public class WindsorAspectsContainer : AspectsContainer
    {
        private readonly IKernel _kernel;

        public WindsorAspectsContainer(IWindsorContainer container)
        {
            _kernel = container.Kernel;
        }

        /// <inheritdoc />
        public override void BindAspects(IAspectsWeavingSettings settings)
        {
            var interceptor = new AspectWeaverInterceptorAdapter(settings);
            var serviceProvider = new Dependency.WindsorServiceProvider(_kernel);

            _kernel.Register(
                Component
                    .For<AspectWeaverInterceptorAdapter>()
                    .Instance(interceptor));

            _kernel.Register(
                Component
                    .For<IServiceProvider>()
                    .Instance(serviceProvider));

            _kernel.AddFacility(new WindsorAspectFacility(settings));
        }

        /// <inheritdoc />
        public override IServiceProvider GetServiceProvider()
        {
            return _kernel.Resolve<IServiceProvider>();
        }
    }
}