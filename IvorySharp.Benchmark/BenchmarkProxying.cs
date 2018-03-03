using System;
using BenchmarkDotNet.Attributes;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Benchmark.Fakes;
using IvorySharp.Core;
using IvorySharp.Proxying;

namespace IvorySharp.Benchmark
{
    internal class DummyConfigurations : IWeavingAspectsConfiguration
    {
        public Type ExplicitWeaingAttributeType { get; } = null;
    }
    
    internal class BypassInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    internal class BypassAspect : MethodBoundaryAspect
    {
    }

    public class BenchmarkProxying
    {
        private IAppService _serviceInstance;
        private IAppService _bypassProxyInstance;
        private IAppService _bypassWeavedInstance;

        private IInterceptor _interceptor;
        private AspectWeaver _aspectWeaver;
        private InterceptProxyGenerator _generator;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _generator = InterceptProxyGenerator.Default;
            _interceptor = new BypassInterceptor();
            _aspectWeaver = new AspectWeaver(new DummyConfigurations());

            _serviceInstance = new AppService();
            _bypassProxyInstance =
                _generator.CreateInterceptProxy<IAppService>(new AppService(), new BypassInterceptor());
            _bypassWeavedInstance = (IAppService) _aspectWeaver.Weave(new AppService(), typeof(IAppService));
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

        [Benchmark]
        public void Dispatch_Class_VoidMethod()
        {
            _serviceInstance.VoidMethod();
        }

        [Benchmark]
        public void Dispatch_NetCoreInvocationProxy_VoidMethod()
        {
            _bypassProxyInstance.VoidMethod();
        }

        [Benchmark]
        public void Dispath_WeavedClass_VoidMethod()
        {
            _bypassWeavedInstance.VoidMethod();
        }
    }
}