using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Провайдер зависимостей аспекта.
    /// </summary>
    internal class DefaultAspectDependencyProvider : IAspectDependencyProvider
    {
        /// <inheritdoc />
        public IEnumerable<AspectPropertyDependency> GetPropertyDependencies(Type aspectType)
        {
            var properties = aspectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var property in properties)
            {
                if (!property.CanWrite || property.GetSetMethod(nonPublic: false) == null)
                    continue;

                var aspectDependency = property
                    .GetCustomAttributes<DependencyAttribute>(inherit: false)
                    .FirstOrDefault();
                
                if (aspectDependency == null)
                    continue;

                yield return new AspectPropertyDependency(aspectDependency, property);
            }
        }
    }
}