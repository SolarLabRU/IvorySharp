using System;
using IvorySharp.Aspects.Configuration;

namespace IvorySharp.Tests.WeavingSettings
{
    public class ImpliticAspectsWeavingSettings : IWeavingAspectsConfiguration
    {
        public Type ExplicitWeaingAttributeType { get; } = null;
    }
}