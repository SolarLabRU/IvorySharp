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
        public bool HasAnyAspect(Type type, bool includeAbstract)
        {
            return SelectAspectDeclarations<MethodAspect>(type, includeAbstract).Any();
        }

        /// <inheritdoc />
        public bool HasAnyAspect(MethodInfo method, bool includeAbstract)
        {
            return SelectAspectDeclarations<MethodAspect>(method, includeAbstract).Any();
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(Type type, bool includeAbstract)
            where TAspect : MethodAspect
        {
            var aspects = type.GetCustomAttributes<TAspect>(inherit: false);

            if (!includeAbstract)
                aspects = aspects.Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromType(a, type));
        }

        /// <inheritdoc />
        public IEnumerable<MethodAspectDeclaration<TAspect>> SelectAspectDeclarations<TAspect>(MethodInfo method, bool includeAbstract)
            where TAspect : MethodAspect
        {
            var aspects = method.GetCustomAttributes<TAspect>(inherit: false);
                
            if (!includeAbstract)
                aspects = aspects.Where(a => !a.GetType().IsAbstract);

            return aspects.Select(a => MethodAspectDeclaration<TAspect>.FromMethod(a, method));
        }
    }
}