using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.CastleWindsor.Aspects.Weaving;
using IvorySharp.Components.Dependency;
using IvorySharp.Configuration;
using IvorySharp.Integration;

namespace IvorySharp.CastleWindsor.Aspects.Integration
{
    /// <summary>
    /// Контейнер аспектов.
    /// </summary>
    public class WindsorAspectsContainer : AspectsContainer
    {
        private readonly IKernel _kernel;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="WindsorAspectsContainer"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public WindsorAspectsContainer(IWindsorContainer container)
        {
            _kernel = container.Kernel;
            _serviceProvider = new Dependency.WindsorServiceProvider(_kernel);
        }

        /// <inheritdoc />
        public override void BindAspects(IAspectsWeavingSettings settings)
        {
            var interceptor = new AspectWeaverInterceptorAdapter(settings);

            _kernel.Register(
                Component
                    .For<AspectWeaverInterceptorAdapter>()
                    .Instance(interceptor));

            _kernel.AddFacility(new WindsorAspectFacility(settings));
        }

        /// <inheritdoc />
        public override IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
    }
}