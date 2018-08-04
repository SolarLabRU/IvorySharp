using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Aspects.Components.Selection
{
    /// <summary>
    /// Компонент выбора аспектов по умолчанию.
    /// </summary>
    internal class DefaultAspectSelector : IAspectSelector
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
        public MethodAspectDeclaration<TAspect>[] SelectAspectDeclarations<TAspect>(Type type, bool includeAbstract)
            where TAspect : MethodAspect
        {
            var declarations = new List<MethodAspectDeclaration<TAspect>>();
            var aspects = type.GetCustomAttributes<TAspect>(inherit: false);

            if (!includeAbstract)
                aspects = aspects.Where(a => !a.GetType().IsAbstract);

            declarations.AddRange(aspects.Select(
                a => MethodAspectDeclaration<TAspect>.FromType(a, type)));

            return declarations.ToArray();
        }

        /// <inheritdoc />
        public MethodAspectDeclaration<TAspect>[] SelectAspectDeclarations<TAspect>(MethodInfo method, bool includeAbstract)
            where TAspect : MethodAspect
        {
            var declarations = new List<MethodAspectDeclaration<TAspect>>();
            var aspects = method.GetCustomAttributes<TAspect>(inherit: false);
                
            if (!includeAbstract)
                aspects = aspects.Where(a => !a.GetType().IsAbstract);

            declarations.AddRange(aspects.Select(
                a => MethodAspectDeclaration<TAspect>.FromMethod(a, method)));

            return declarations.ToArray();
        }
    }
}