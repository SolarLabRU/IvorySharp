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
        private readonly IDependencyProvider _dependencyProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="WindsorAspectsContainer"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public WindsorAspectsContainer(IWindsorContainer container)
        {
            _kernel = container.Kernel;
            _dependencyProvider = new Dependency.WindsorDependencyProvider(_kernel);
        }

        /// <inheritdoc />
        public override void BindAspects(IComponentsStore settings)
        {
            var interceptor = new WeavedInterceptor(settings);

            _kernel.Register(
                Component
                    .For<WeavedInterceptor>()
                    .Instance(interceptor));

            _kernel.AddFacility(new WindsorAspectFacility(settings));
        }

        /// <inheritdoc />
        public override IDependencyProvider GetDependencyProvider()
        {
            return _dependencyProvider;
        }
    }
}