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
        public override void BindAspects(IWeavingAspectsConfiguration configuration)
        {
            var interceptor = new AspectWeaverInterceptorAdapter(configuration);

            _kernel.Register(
                Component
                    .For<AspectWeaverInterceptorAdapter>()
                    .Instance(interceptor));

            _kernel.AddFacility(new WindsorAspectFacility(configuration));
        }

        /// <inheritdoc />
        public override IServiceProvider GetServiceProvider()
        {
            throw new System.NotImplementedException();
        }
    }
}