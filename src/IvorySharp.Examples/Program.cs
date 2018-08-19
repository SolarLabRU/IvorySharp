using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Examples.AppServices;
using IvorySharp.Examples.Models;
using IvorySharp.Integration.SimpleInjector.Aspects.Integration;
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

            for (int i = 1; i < 4; i++)
            {
                var customer = service.GetCustomer(i);
            }

            Console.ReadLine();
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