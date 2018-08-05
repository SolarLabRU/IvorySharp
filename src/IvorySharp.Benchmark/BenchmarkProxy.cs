using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Castle.DynamicProxy;
using IvorySharp.Caching;
using IvorySharp.Comparers;
using IvorySharp.Proxying;
using IvorySharp.Reflection;
using DispatchProxy = System.Reflection.DispatchProxy;
using ProxyGenerator = IvorySharp.Proxying.ProxyGenerator;

namespace IvorySharp.Benchmark
{
    public class BenchmarkProxy
    {
        private static Castle.DynamicProxy.ProxyGenerator _castleProxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
        
        private ITestService _ivoryProxy;
        private ITestService _dispatchProxy;
        private ITestService _castleProxy;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            _ivoryProxy = IvoryProxy<ITestService>.CreateProxy(new TestService());
            _dispatchProxy = DispatchProxy<ITestService>.CreateProxy(new TestService());
            _castleProxy = _castleProxyGenerator.CreateInterfaceProxyWithTarget<ITestService>(
                new TestService(), new BypassCastleInterceptor());
        }

        [Benchmark]
        public void InvokeCastleProxy()
        {
            var result = _castleProxy.Identity(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public void InvokeIvoryProxy()
        {
            var result = _ivoryProxy.Identity(10);
            GC.KeepAlive(result);
        }

        [Benchmark]
        public void InvokeDispatchProxy()
        {
            var result = _dispatchProxy.Identity(10);
            GC.KeepAlive(result);
        }
        
        public interface ITestService
        {
            int Identity(int number);
        }     
        
        public class TestService : ITestService
        {
            public int Identity(int number)
            {
                return number;
            }
        }
        
        public class IvoryProxy<TService> : IvoryProxy
        {
            private TService _service;
            private Func<MethodInfo, Func<object, object[], object>> _methodInvokerFactory;
            
            public static TService CreateProxy<TImpementation>(TImpementation instance) where TImpementation : TService
            {
                var proxy = ProxyGenerator.Instance.CreateTransparentProxy(typeof(IvoryProxy<TService>),
                    typeof(TService));

                var castedProxy = (IvoryProxy<TService>) proxy;
                
                castedProxy._service = instance;
                castedProxy._methodInvokerFactory = Memoizer.CreateProducer(
                    Expressions.CreateMethodInvoker, MethodEqualityComparer.Instance);
                
                return (TService)proxy;
            }

            protected internal override object Invoke(MethodInfo targetMethod, object[] args)
            {
                return _methodInvokerFactory(targetMethod)(_service, args);
            }
        }
        
        public class DispatchProxy<TService> : DispatchProxy
        {
            private TService _service;
            
            public static TService CreateProxy<TImpementation>(TImpementation instance) where TImpementation : TService
            {
                var proxy = Create<TService, DispatchProxy<TService>>();

                var castedProxy = proxy as DispatchProxy<TService>;
                castedProxy._service = instance;
                return (TService)proxy;
            }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                return targetMethod.Invoke(_service, args);
            }
        }
        
        public class BypassCastleInterceptor : Castle.DynamicProxy.IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
            }
        }
    }
}