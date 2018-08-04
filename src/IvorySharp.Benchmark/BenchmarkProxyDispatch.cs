using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Benchmark.Fakes;
using IvorySharp.Proxying;

namespace IvorySharp.Benchmark
{
    public class BenchmarkProxyDispatch
    {
        private IAppService _serviceInstance;
        private IAppService _bypassProxyInstance;
        private IAppService _bypassWeavedInstance;
        private IDependencyAppService _dependencyAppService;
        private DummyComponents _dummyComponents;
     
        [GlobalSetup]
        public void GlobalSetup()
        {
            var proxyGenerator = InterceptProxyGenerator.Default;
            var serviceContainer = new Dictionary<Type, object>
            {
                [typeof(IDependencyService)] = new DependencyService()
            };
            
            var dependencyProvider = new DummyDependencyProvider(serviceContainer);
            _dummyComponents = new DummyComponents {DependencyProvider = dependencyProvider};

            var weaver = new AspectWeaver(_dummyComponents.AspectWeavePredicate, _dummyComponents.AspectPipelineExecutor, _dummyComponents.AspectFactory);

            _serviceInstance = new AppService();
            _bypassProxyInstance = proxyGenerator.CreateInterceptProxy<IAppService, AppService>(new AppService(), new BypassInterceptor());
            _bypassWeavedInstance = (IAppService) weaver.Weave(new AppService(), typeof(IAppService), typeof(AppService));
            _dependencyAppService = (IDependencyAppService) weaver.Weave(new DependencyAppService(), typeof(IDependencyAppService), typeof(DependencyAppService));
         }


        [Benchmark]
        public void Dispatch_Class_IdentityMethod()
        {
            _serviceInstance.Identity(10);
        }

        [Benchmark]
        public void Dispatch_Proxy_IdentityMethod()
        {
            _bypassProxyInstance.Identity(10);
        }

        [Benchmark]
        public void Dispatch_WeavedClass_IdentityMethod()
        {
            _bypassWeavedInstance.Identity(10);
        }

        [Benchmark]
        public void Dispatch_WeavedDependencyClass_IdentityMethod()
        {
            _dependencyAppService.Identity(10);
        }
        
        [Benchmark]
        public void Dispath_Reflection_IdentityMethod()
        {
            var method = typeof(AppService).GetMethod("Identity");
            method.Invoke(_serviceInstance, new object[]{ 10 });
        }
    }
}