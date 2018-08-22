using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Aspects.Selection
{
    /// <summary>
    /// Компонент выбора аспектов по умолчанию.
    /// </summary>
    internal sealed class DefaultAspectSelector : IAspectSelector
    {
        /// <inheritdoc />
        public bool HasAnyAspect(Type type)
        {
            return SelectAspectDeclarations<MethodAspect>(type).Any();
        }

        /// <inheritdoc />
        public bool HasAnyAspect(MethodInfo method)
        {
            return SelectAspectDeclarations<MethodAspect>(method).Any();
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(Type type)
            where TAspect : MethodAspect
        {
            var aspects = type.GetCustomAttributes<TAspect>(inherit: false)
                .Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromType(a, type));
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(MethodInfo method)
            where TAspect : MethodAspect
        {
            var aspects = method.GetCustomAttributes<TAspect>(inherit: false)
                .Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromMethod(a, method));
        }
    }
}