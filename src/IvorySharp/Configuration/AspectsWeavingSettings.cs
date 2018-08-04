using System;
using IServiceProvider = IvorySharp.Components.Dependency.IServiceProvider;

namespace IvorySharp.Configuration
{
    /// <summary>
    /// Модель настроек по внедрению аспектов.
    /// </summary>
    internal class AspectsWeavingSettings : IAspectsWeavingSettings
    {
        /// <inheritdoc />
        public Type ExplicitWeavingAttributeType { get; set; }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; set; }
    }
}