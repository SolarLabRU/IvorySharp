using System;
using IvorySharp.Aspects.Configuration;
using IServiceProvider = IvorySharp.Aspects.Components.Dependency.IServiceProvider;

namespace IvorySharp.Benchmark.Fakes
{
    internal class DummyConfigurations : IComponentsStore
    {
        public Type ExplicitWeavingAttributeType { get; set; } = null;
        public IServiceProvider ServiceProvider { get; set; } = null;
    }
}