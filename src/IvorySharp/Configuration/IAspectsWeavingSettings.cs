using System;
using IServiceProvider = IvorySharp.Components.Dependency.IServiceProvider;

namespace IvorySharp.Configuration
{
    /// <summary>
    /// Настройки по внедрению аспектов.
    /// </summary>
    public interface IAspectsWeavingSettings
    {
        /// <summary>
        /// Атрибут, которым должны помечаться все сервисы,
        /// для которых включена компоновка аспектов.
        /// </summary>
        Type ExplicitWeavingAttributeType { get; }
        
        /// <summary>
        /// Провайдер сервисов.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}