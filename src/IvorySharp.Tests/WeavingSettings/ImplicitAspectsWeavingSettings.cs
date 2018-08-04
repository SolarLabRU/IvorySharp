using System;
using IvorySharp.Configuration;
using IServiceProvider = IvorySharp.Components.Dependency.IServiceProvider;

namespace IvorySharp.Tests.WeavingSettings
{
    public class ImplicitAspectsWeavingSettings : IAspectsWeavingSettings
    {
        public Type ExplicitWeavingAttributeType { get; } = null;
        public IServiceProvider ServiceProvider { get; } = null;
    }
}