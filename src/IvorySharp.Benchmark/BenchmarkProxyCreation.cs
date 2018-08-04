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

        [GlobalSetup]
        public void GlobalSetup()
        {
            _generator = InterceptProxyGenerator.Default;
            _interceptor = new BypassInterceptor();
            _aspectWeaver = new AspectWeaver(new DummyConfigurations());
        }


        [Benchmark]
        public void Create_Class_Weaved()
        {
            IAppService service = (IAppService) _aspectWeaver.Weave(new AppService(), typeof(IAppService));

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
            var service = _generator.CreateInterceptProxy<IAppService>(new AppService(), _interceptor);

            GC.KeepAlive(service);
        }
    }
}