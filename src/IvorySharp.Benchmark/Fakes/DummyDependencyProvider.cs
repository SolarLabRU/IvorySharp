using System;
using System.Collections.Generic;
using System.Linq;

namespace IvorySharp.Benchmark.Fakes
{
    public class DummyDependencyProvider : Aspects.Components.Dependency.IDependencyProvider
    {
        private Dictionary<Type, object> _container;

        public DummyDependencyProvider(Dictionary<Type, object> container)
        {
            _container = container;
        }

        public TService GetService<TService>() where TService : class
        {
            return _container.FirstOrDefault(p => p.Key == typeof(TService)).Value as TService;
        }

        public TService GetTransparentService<TService>() where TService : class
        {
            return _container.FirstOrDefault(p => p.Key == typeof(TService)).Value as TService;
        }

        public TService GetNamedService<TService>(string key) where TService : class
        {
            return _container.FirstOrDefault(p => p.Key == typeof(TService)).Value as TService;
        }

        public TService GetTransparentNamedService<TService>(string key) where TService : class
        {
            return _container.FirstOrDefault(p => p.Key == typeof(TService)).Value as TService;
        }

        public object GetService(Type serviceType)
        {
            return _container.FirstOrDefault(p => p.Key == serviceType).Value;
        }

        public object GetTransparentService(Type serviceType)
        {
            return _container.FirstOrDefault(p => p.Key == serviceType).Value;
        }

        public object GetNamedService(Type serviceType, string key)
        {
            return _container.FirstOrDefault(p => p.Key == serviceType).Value;
        }

        public object GetTransparentNamedService(Type serviceType, string key)
        {
            return _container.FirstOrDefault(p => p.Key == serviceType).Value;
        }
    }
}