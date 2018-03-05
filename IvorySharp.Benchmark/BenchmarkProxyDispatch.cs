using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Benchmark.Fakes;
using IvorySharp.Proxying;

namespace IvorySharp.Benchmark
{
    [CoreJob]
    public class BenchmarkProxyDispatch
    {
        private IAppService _serviceInstance;
        private IAppService _bypassProxyInstance;
        private IAppService _bypassWeavedInstance;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _serviceInstance = new AppService();
            _bypassProxyInstance =
                InterceptProxyGenerator.Default.CreateInterceptProxy<IAppService>(new AppService(), new BypassInterceptor());
            _bypassWeavedInstance = (IAppService) new AspectWeaver(new DummyConfigurations()).Weave(new AppService(), typeof(IAppService));
        }


        [Benchmark]
        public void Dispatch_Class_IdentityMethod()
        {
            _serviceInstance.Identity(10);
        }

        [Benchmark]
        public void Dispatch_NetCoreInvocationProxy_IdentityMethod()
        {
            _bypassProxyInstance.Identity(10);
        }

        [Benchmark]
        public void Dispath_WeavedClass_IdentityMethod()
        {
            _bypassWeavedInstance.Identity(10);
        }
        
        [Benchmark]
        public void Dispath_Reflection_IdentityMethod()
        {
            var method = typeof(AppService).GetMethod("Identity");
            method.Invoke(_serviceInstance, new object[]{ 10 });
        }
    }
}