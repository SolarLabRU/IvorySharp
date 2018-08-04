using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IvorySharp.Aspects.Components.Selection
{
    /// <summary>
    /// Стратегия выбора аспектов по умолчанию.
    /// </summary>
    internal class DefaultMethodAspectSelectionStrategy : IMethodAspectSelectionStrategy
    {
        /// <inheritdoc />
        public bool HasAnyAspect(Type type, bool includeAbstract)
        {
            return GetDeclarations<MethodAspect>(type, includeAbstract).Any();
        }

        /// <inheritdoc />
        public bool HasAnyAspect(MethodInfo method, bool includeAbstract)
        {
            return GetDeclarations<MethodAspect>(method, includeAbstract).Any();
        }

        /// <inheritdoc />
        public MethodAspectDeclaration<TAspect>[] GetDeclarations<TAspect>(Type type, bool includeAbstract)
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
        public MethodAspectDeclaration<TAspect>[] GetDeclarations<TAspect>(MethodInfo method, bool includeAbstract)
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