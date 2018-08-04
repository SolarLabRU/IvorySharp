using System;
using IvorySharp.Configuration;
using IServiceProvider = IvorySharp.Components.Dependency.IServiceProvider;

namespace IvorySharp.Benchmark.Fakes
{
    internal class DummyConfigurations : IAspectsWeavingSettings
    {
        public Type ExplicitWeavingAttributeType { get; set; } = null;
        public IServiceProvider ServiceProvider { get; set; } = null;
    }
}