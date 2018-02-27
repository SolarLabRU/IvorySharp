using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using Castle.DynamicProxy;
using IvoryProxy.Core;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Core.Interceptors;
using IInterceptor = Castle.Core.Interceptor.IInterceptor;

namespace IvoryProxy.Benchmark
{
    public class AllowNonOptimized : ManualConfig
    {
        public AllowNonOptimized()
        {
            Add(JitOptimizationsValidator.DontFailOnError); // ALLOW NON-OPTIMIZED DLLS

            Add(DefaultConfig.Instance.GetLoggers().ToArray()); // manual config has no loggers by default
            Add(DefaultConfig.Instance.GetExporters().ToArray()); // manual config has no exporters by default
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray()); // manual config has no columns by default
        }
    }
    
    public class InterfaceProxyBenchmark
    {
           
        private IService _serviceInstance;
        private IService _proxyInstance;
        private IService _castleProxyInstance;

        private IProxyGenerator _ivoryProxyGenerator;
        private ProxyGenerator _castleProxyGenerator;
        
        [GlobalSetup]
        public void Setup()
        {
            _ivoryProxyGenerator = new IvoryProxyGenerator();
            _serviceInstance = new Service();
            _proxyInstance = IvoryProxyGenerator.Default.CreateInterfaceProxy<IService>(new Service()).TransparentProxy;
            _castleProxyGenerator = new ProxyGenerator();
            _castleProxyInstance = _castleProxyGenerator.CreateInterfaceProxyWithTarget<IService>(new Service(), ProxyGenerationOptions.Default,
                new BypassCastleInterceptor());
        }
        
        [Benchmark]
        public void DispatchVoidMethod_NoProxy()
        {
            _serviceInstance.VoidMethod();
        }

        [Benchmark]
        public void DispatchVoidMethod_Proxy()
        {
            _proxyInstance.VoidMethod();
        }

        [Benchmark]
        public void DispatchVoidMethod_CastleProxy()
        {
            _castleProxyInstance.VoidMethod();
        }
        
        [Benchmark]
        public void Instantiate_NewService_DefaultCtor()
        {
            var service = new Service();
            GC.KeepAlive(service);
        }

        [Benchmark]
        public void Instantiate_DefaultProxy()
        {
            IService service = new Service();
            var proxy = _ivoryProxyGenerator.CreateInterfaceProxy<IService>(service).TransparentProxy;
            
            GC.KeepAlive(proxy);
        }

        [Benchmark]
        public void Instantiate_CastleProxy()
        {
            IService service = new Service();
            var proxy = _castleProxyGenerator.CreateInterfaceProxyWithTarget(service, ProxyGenerationOptions.Default,
                new BypassCastleInterceptor());
            
            GC.KeepAlive(proxy);
        }
    }


    public interface IService
    {
        [Intercept(typeof(BypassInterceptor))]
        void VoidMethod();
    }

    public class Service : IService
    {
        public void VoidMethod()
        {
        }
    }

    public class BypassInterceptor : IvoryInterceptor
    {
        public override void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class BypassCastleInterceptor : IInterceptor
    {
        public void Intercept(Castle.Core.Interceptor.IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}