using System;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.Aspects.Configuration
{
    /// <summary>
    /// Настройки по внедрению аспектов.
    /// </summary>
    public interface IWeavingAspectsConfiguration
    {
        /// <summary>
        /// Атрибут, которым должны помечаться все сервисы,
        /// для которых включена компоновка аспектов.
        /// </summary>
        Type ExplicitWeaingAttributeType { get; }
        
        /// <summary>
        /// Провайдер сервисов.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}