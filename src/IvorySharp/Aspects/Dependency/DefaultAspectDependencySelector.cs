using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Селектор зависимостей аспектов.
    /// </summary>
    internal sealed class DefaultAspectDependencySelector : IAspectDependencySelector
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AspectPropertyDependency[] SelectPropertyDependencies(Type aspectType)
        {
            var dependencies = new List<AspectPropertyDependency>();
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

                dependencies.Add(new AspectPropertyDependency(aspectDependency, property));
            }

            return dependencies.ToArray();
        }
    }
}