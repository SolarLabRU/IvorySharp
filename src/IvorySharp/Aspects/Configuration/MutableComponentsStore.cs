using System;
using IServiceProvider = IvorySharp.Aspects.Components.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Компоненты библиотеки.
    /// </summary>
    internal class MutableComponentsStore : IComponentsStore
    {
        /// <inheritdoc />
        public Type ExplicitWeavingAttributeType { get; set; }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; set; }
    }
}