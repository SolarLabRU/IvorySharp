using System;
using IvorySharp.Aspects.Configuration;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Tests.WeavingSettings
{
    public class ImpliticAspectsAspectsWeavingSettings : IAspectsWeavingSettings
    {
        public Type ExplicitWeavingAttributeType { get; } = null;
        public IServiceProvider ServiceProvider { get; } = null;
    }
}