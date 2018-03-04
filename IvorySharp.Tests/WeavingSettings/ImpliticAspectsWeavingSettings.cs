using System;
using IvorySharp.Aspects.Configuration;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Tests.WeavingSettings
{
    public class ImpliticAspectsWeavingSettings : IAspectsWeavingSettings
    {
        public Type ExplicitWeavingAttributeType { get; } = null;
        public IServiceProvider ServiceProvider { get; } = null;
    }

    public class ExplicitAspectWeavingSettings : IAspectsWeavingSettings
    {
        public Type ExplicitWeavingAttributeType { get; } = typeof(EnableWeavingAttribute);
        public IServiceProvider ServiceProvider { get; }
    }
    
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class EnableWeavingAttribute : Attribute { }
}