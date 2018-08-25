using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент выбора аспектов по умолчанию.
    /// </summary>
    internal sealed class DefaultAspectSelector : IAspectSelector
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAnyAspect(Type type)
        {
            return type.CustomAttributes
                .Any(a => typeof(MethodAspect).IsAssignableFrom(a.AttributeType));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasAnyAspect(MethodInfo method)
        {
            return method.CustomAttributes
                .Any(a => typeof(MethodAspect).IsAssignableFrom(a.AttributeType));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(Type type)
            where TAspect : MethodAspect
        {
            var aspects = type.GetCustomAttributes<TAspect>(inherit: false)
                .Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromType(a, type));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(MethodInfo method)
            where TAspect : MethodAspect
        {
            var aspects = method.GetCustomAttributes<TAspect>(inherit: false)
                .Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromMethod(a, method));
        }
    }
}