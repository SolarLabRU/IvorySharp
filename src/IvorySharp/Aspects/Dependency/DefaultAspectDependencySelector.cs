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

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var property in GetPropertyDependencies(aspectType))
            {
                var aspectDependency = property
                    .GetCustomAttributes<DependencyAttribute>(inherit: false)
                    .First();
                
                dependencies.Add(new AspectPropertyDependency(aspectDependency, property));
            }

            return dependencies.ToArray();
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasDependencies(Type aspectType)
        {
            return GetPropertyDependencies(aspectType).Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<PropertyInfo> GetPropertyDependencies(Type aspectType)
        {
            return aspectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite &&
                            p.GetSetMethod(nonPublic: false) != null &&
                            p.CustomAttributes.Any(a => a.AttributeType == typeof(DependencyAttribute)));
        }
    }
}