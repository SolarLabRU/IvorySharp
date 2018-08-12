using System;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Benchmark.Proxy;
using IvorySharp.Benchmark.Services;
using IvorySharp.CastleWindsor.Aspects.Integration;

namespace IvorySharp.Benchmark
{
    public class BenchmarkDispatch
    {
        private IServiceForBenchmark _proxiedService;
        private IServiceForBenchmark _weavedService;
        private IServiceForBenchmark _windsorService;
        
        private MethodInfo _reflectedMethod;
        private ServiceForBenchmark _reflectedMethodService;

        [GlobalSetup]
        public void Setup()
        {
            var winsorContainer = new WindsorContainer();
            
            AspectsConfigurator
                .UseContainer(new WindsorAspectsContainer(winsorContainer))
                .Initialize();

            winsorContainer.Register(
                Component
                    .For<IServiceForBenchmark>()
                    .ImplementedBy<ServiceForBenchmark>());

            _windsorService = winsorContainer.Resolve<IServiceForBenchmark>();
            
            _reflectedMethodService = new ServiceForBenchmark();
            _reflectedMethod = typeof(IServiceForBenchmark).GetMethod(nameof(IServiceForBenchmark.Identity));

            var settings = new DefaultComponentsStore(null);
            var weaver = new AspectWeaver(settings.AspectWeavePredicate, settings.PipelineFactory, settings.AspectFactory);
            
            _weavedService = (IServiceForBenchmark) weaver.Weave(
                new ServiceForBenchmark(), typeof(IServiceForBenchmark), typeof(ServiceForBenchmark));
            
            _proxiedService = (IServiceForBenchmark) BypassProxy<IServiceForBenchmark>.Create(
                new ServiceForBenchmark());
        }
        
        [Benchmark]
        public void DispatchProxiedMethod()
        {
            var result = _proxiedService.Identity(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public void DispatchWeavedMethod()
        {
            var result = _weavedService.Identity(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public void DispatchWindsorMethod()
        {
            var result = _windsorService.Identity(10);
            GC.KeepAlive(result);
        }
        
        [Benchmark]
        public void DispatchReflectedMethod()
        {
            var result = _reflectedMethod.Invoke(_reflectedMethodService, new object[]{ 10 });
            GC.KeepAlive(result);
        }

        [Benchmark]
        public async Task DispatchWeavedAsyncMethod()
        {
            var result = await _weavedService.IdentityAsync(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public async Task DispatchWindsorAsyncMethod()
        {
            var result = await _windsorService.IdentityAsync(10);
            GC.KeepAlive(result);
        }
    }
}