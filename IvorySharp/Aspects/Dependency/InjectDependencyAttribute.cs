using System;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Атрибут для обозначения свойст, которые должны быть внедрены как зависимости аспекта.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class InjectDependencyAttribute : Attribute
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