using System;
using IvorySharp.Aspects.Dependency;

namespace IvorySharp.Benchmark
{
    public class NullDependencyProvider : IDependencyProvider
    {
        public TService GetService<TService>() where TService : class
        {
            return null;
        }

        public TService GetTransparentService<TService>() where TService : class
        {
            return null;
        }

        public TService GetNamedService<TService>(string key) where TService : class
        {
            return null;
        }

        public TService GetTransparentNamedService<TService>(string key) where TService : class
        {
            return null;
        }

        public object GetService(Type serviceType)
        {
            return null;
        }

        public object GetTransparentService(Type serviceType)
        {
            return null;
        }

        public object GetNamedService(Type serviceType, string key)
        {
            return null;
        }

        public object GetTransparentNamedService(Type serviceType, string key)
        {
            return null;
        }
    }
}