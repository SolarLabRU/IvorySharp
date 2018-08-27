using System;
using BenchmarkDotNet.Attributes;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Benchmark.Services;

namespace IvorySharp.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkCreation
    {
        private AspectWeaver _weaver;
        private WindsorContainer _container;
        
        [GlobalSetup]
        public void Setup()
        {
            _weaver = AspectWeaverFactory.Create();
            _container = new WindsorContainer();

            _container.Register(
                Component.For<IServiceForBenchmark>()
                    .ImplementedBy<ServiceForBenchmark>());
        }

        [Benchmark]
        public void CreateUsingActivator()
        {
            var service = (IServiceForBenchmark) Activator.CreateInstance(typeof(ServiceForBenchmark));
            GC.KeepAlive(service);
        }

        [Benchmark]
        public void CreateUsingWindsorResolve()
        {
            var service = _container.Resolve<IServiceForBenchmark>();
            GC.KeepAlive(service);
        }

        [Benchmark]
        public void CreateWeavedUsingNativeWeaver()
        {
            var service = _weaver.Weave<IServiceForBenchmark, ServiceForBenchmark>(
                new ServiceForBenchmark());
            
            GC.KeepAlive(service);
        }
    }
}