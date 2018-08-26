using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Integration.CastleWindsor.Aspects.Integration;

namespace Examples.ExceptionHandling
{
    public sealed class Program
    {
        private static WindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(container))
                .Initialize();

            container.Register(
                Component.For<IDataService>()
                    .ImplementedBy<DataService>());

            return container;
        }
        
        static void Main(string[] args)
        {
            var container = CreateContainer();
            var service = container.Resolve<IDataService>();
            
            service.GenerateData();
        }
    }
}