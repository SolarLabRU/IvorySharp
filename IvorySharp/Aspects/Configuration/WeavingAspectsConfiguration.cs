using System;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Модель настроек по внедрению аспектов аспектов.
    /// </summary>
    internal class WeavingAspectsConfiguration : IWeavingAspectsConfiguration
    {
        /// <inheritdoc />
        public Type ExplicitWeaingAttributeType { get; set; }
    }
}