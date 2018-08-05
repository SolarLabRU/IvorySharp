using System;
using BenchmarkDotNet.Attributes;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Benchmark.Fakes;

namespace IvorySharp.Benchmark
{
    public class BenchmarkProxyCreation
    {
        private AspectWeaver _aspectWeaver;
        private DummyComponents _dummyComponents;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _dummyComponents = new DummyComponents();
            _aspectWeaver = new AspectWeaver(_dummyComponents.AspectWeavePredicate, _dummyComponents.AspectPipelineExecutor, _dummyComponents.AspectFactory);
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
    }
}