using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Integration.SimpleInjector.Aspects.Integration;
using SimpleInjector;

namespace Examples.Caching
{
    public sealed class Program
    {
        private static Container CreateContainer()
        {
            var container = new Container();

            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(container))
                .Initialize();
            
            container.Register<IPersonService, PersonService>();

            return container;
        }
        
        static void Main(string[] args)
        {
            var container = CreateContainer();
            var service = container.GetInstance<IPersonService>();

            service.GetPerson(0);
            service.GetPerson(1);

            service.GetPerson(0);
            service.GetPerson(1);
        }
    }
}