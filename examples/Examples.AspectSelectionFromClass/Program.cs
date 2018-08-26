using IvorySharp.Aspects.Configuration;
using IvorySharp.Extensions.ClassAspectSelection;
using IvorySharp.Integration.SimpleInjector.Aspects.Integration;
using SimpleInjector;

namespace Examples.AspectSelectionFromClass
{
    class Program
    {
        private static Container CreateContainer()
        {
            var container = new Container();

            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(container))
                .Initialize(cfg =>
                {
                    cfg.UseClassAspectSelection();
                });
            
            container.Register<IDataService, DataService>();

            return container;
        }
        
        static void Main(string[] args)
        {
            var container = CreateContainer();
            var service = container.GetInstance<IDataService>();

            service.GenerateData();
        }
    }
}