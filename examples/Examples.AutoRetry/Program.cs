using System;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Integration.CastleWindsor.Aspects.Integration;

namespace Examples.AutoRetry
{
    class Program
    {
        private static WindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();

            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(container))
                .Initialize();

            container.Register(
                Component.For<IWebPageService>()
                    .ImplementedBy<WebPageService>());

            return container;
        }
        
        static void Main(string[] args)
        {
            var container = CreateContainer();
            var service = container.Resolve<IWebPageService>();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                
                try
                {
                    Console.WriteLine($"\n Getting page: SomePage_{i}");
                    service.GetPageContent($"SomePage_{i}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\n Unable to get page: SomePage_{i}: {e.Message}");
                }
            }
        }
    }
}