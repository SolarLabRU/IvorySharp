using System;
using IvorySharp.Aspects.Configuration;
using IServiceProvider = IvorySharp.Aspects.Components.Dependency.IServiceProvider;

namespace IvorySharp.Tests.WeavingSettings
{
    public class NullComponentsStore : IComponentsStore
    {
        public Type ExplicitWeavingAttributeType { get; } = null;
        public IServiceProvider ServiceProvider { get; } = null;
    }
}