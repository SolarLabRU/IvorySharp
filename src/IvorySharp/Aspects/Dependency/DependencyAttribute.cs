using System;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Атрибут для обозначения свойств, которые должны быть внедрены как зависимости аспекта.
    /// </summary>
    [PublicAPI, AttributeUsage(AttributeTargets.Property)]
    public sealed class DependencyAttribute : Attribute
    {    
        /// <summary>
        /// Ключ именованного сервиса.
        /// </summary>
        public string ServiceKey { get; set; }
        
        /// <summary>
        /// Признак того, что если зависимость задана как сервис, который
        /// скомпонован аспектами, то в качестве зависимости необходимо получить
        /// оригинальный сервис без применения аспектов.
        /// </summary>
        public bool Transparent { get; set; }
    }
}