using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Examples.AppServices;
using IvorySharp.Examples.Models;
using IvorySharp.SimpleInjector.Aspects.Integration;
using SimpleInjector;

namespace IvorySharp.Examples
{
    public class Program
    {
        private static Container _dependencyContainer;
        
        static void Main(string[] args)
        {
            RegisterServices();
            
            var service = _dependencyContainer.GetInstance<ICustomerService>();
            
            service.CreateCustomer(new Customer {Id = 1, Name = "John", LastName = "Doe"});
            service.CreateCustomer(new Customer {Id = 2, Name = "John", LastName = "Doe"});
            service.CreateCustomer(new Customer {Id = 3, Name = "John", LastName = "Doe"});

            for (int i = 0; i < 10; i++)
            {
                var customer = service.GetCustomer(1);
            }
        }

        private static void RegisterServices()
        {
            _dependencyContainer = new Container();
            
            AspectsConfigurator
                .UseContainer(new SimpleInjectorAspectContainer(_dependencyContainer))
                .Initialize();
            
            _dependencyContainer.Register<ICustomerService, CustomerService>();
        }
    }
}