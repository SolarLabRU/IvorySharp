using System;
using BenchmarkDotNet.Attributes;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Benchmark.Fakes;
using IvorySharp.Core;
using IvorySharp.Proxying;

namespace IvorySharp.Benchmark
{
    public class BenchmarkProxyCreation
    {
        private IInterceptor _interceptor;
        private AspectWeaver _aspectWeaver;
        private InterceptProxyGenerator _generator;
        private DummyComponents _dummyComponents;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _generator = InterceptProxyGenerator.Default;
            _interceptor = new BypassInterceptor();
            _dummyComponents = new DummyComponents();
            _aspectWeaver = new AspectWeaver(_dummyComponents.AspectWeavePredicate, _dummyComponents.AspectPipelineExecutor, _dummyComponents.AspectInitializer);
        }


        [Benchmark]
        public void Create_Class_Weaved()
        {
            IAppService service = (IAppService) _aspectWeaver.Weave(new AppService(), typeof(IAppService), typeof(AppService));

            GC.KeepAlive(service);
        }

        [Benchmark]
        public void Create_Class_Using_New()
        {
            IAppService service = new AppService();

            GC.KeepAlive(service);
        }

        [Benchmark]
        public void Create_NetCoreInvocationProxy()
        {
            var service = _generator.CreateInterceptProxy<IAppService, AppService>(new AppService(), _interceptor);

            GC.KeepAlive(service);
        }
    }
}