using System;

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
    }
}