using System;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настроек по внедрению аспектов.
    /// </summary>
    internal class AspectsWeavingSettings : IWeavingAspectsConfiguration
    {
        /// <inheritdoc />
        public Type ExplicitWeaingAttributeType { get; set; }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; set; }
    }
}