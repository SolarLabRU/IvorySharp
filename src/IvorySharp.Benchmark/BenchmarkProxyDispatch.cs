using System;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Benchmark.Proxy;
using IvorySharp.Benchmark.Services;

namespace IvorySharp.Benchmark
{
    public class BenchmarkDispatch
    {
        private IServiceForBenchmark _defaultService;
        private IServiceForBenchmark _proxiedService;
        private IServiceForBenchmark _weavedService;
        
        private MethodInfo _reflectedMethod;
        private ServiceForBenchmark _reflectedMethodService;

        [GlobalSetup]
        public void Setup()
        {
            _reflectedMethodService = new ServiceForBenchmark();
            _reflectedMethod = typeof(IServiceForBenchmark).GetMethod(nameof(IServiceForBenchmark.Identity));

            var settings = new DefaultComponentsStore(null);
            var weaver = new AspectWeaver(settings.AspectWeavePredicate, settings.PipelineFactory, settings.AspectFactory);
            
            _defaultService = new ServiceForBenchmark();
            
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
        public void DispatchReflectedMethod()
        {
            var result = _reflectedMethod.Invoke(_reflectedMethodService, new object[]{ 10 });
            GC.KeepAlive(result);
        }

        [Benchmark]
        public void DispachMethodDefault()
        {
            var result = _defaultService.Identity(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public async Task DispatchAsyncMethod()
        {
            var result = await _defaultService.IdentityAsync(10);
            GC.KeepAlive(result);
        }
        
        [Benchmark]
        public async Task DispatchWeavedAsyncMethod()
        {
            var result = await _weavedService.IdentityAsync(10);
            GC.KeepAlive(result);
        }
    }
}