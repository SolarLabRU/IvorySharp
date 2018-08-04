using System;
using IServiceProvider = IvorySharp.Aspects.Components.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Настройки по внедрению аспектов.
    /// </summary>
    public interface IComponentsStore
    { 
        /// <summary>
        /// Провайдер сервисов.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}