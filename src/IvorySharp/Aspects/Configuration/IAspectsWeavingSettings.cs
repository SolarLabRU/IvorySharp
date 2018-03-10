using System;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Configuration
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