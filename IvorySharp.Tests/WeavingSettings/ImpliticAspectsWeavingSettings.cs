using System;
using IvorySharp.Aspects.Configuration;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Tests.WeavingSettings
{
    public class ImpliticAspectsWeavingSettings : IWeavingAspectsConfiguration
    {
        public Type ExplicitWeaingAttributeType { get; } = null;
        public IServiceProvider ServiceProvider { get; } = null;
    }
}