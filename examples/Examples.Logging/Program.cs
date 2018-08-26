using IvorySharp.Aspects.Configuration;
using IvorySharp.Integration.SimpleInjector.Aspects.Integration;
using SimpleInjector;

namespace Examples.Logging
{
    public sealed class Program
    {
        private static Container CreateContainer()
        {
            var container = new Container();

            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(container))
                .Initialize();
            
            container.Register<ICustomerService, CustomerService>();

            return container;
        }
        
        static void Main(string[] args)
        {
            var container = CreateContainer();
            var service = container.GetInstance<ICustomerService>();

            service.SaveCustomer(new Customer {FirstName = "John", LastName = "Doe"});
            service.SaveCustomer(new Customer {FirstName = "Nick", LastName = "Cake"});
        }
    }
}