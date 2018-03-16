using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Core;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Реализация фабрики аспектов.
    /// </summary>
    internal class MethodAspectFactory
    {
        /// <summary>
        /// Экземпляр селектора аспектов.
        /// </summary>
        internal static MethodAspectFactory Instance { get; } = new MethodAspectFactory();

        private MethodAspectFactory()
        {
        }

        /// <summary>
        /// Создает аспекты, применимые к контексту вызова.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Коллекция аспектов.</returns>
        public List<MethodBoundaryAspect> CreateMethodBoundaryAspects(InvocationContext context)
        {
            var declaredTypeAspects = context.InstanceDeclaringType
                .GetCustomAttributes<MethodBoundaryAspect>(inherit: false);

            var methodAspects = context.Method
                .GetCustomAttributes<MethodBoundaryAspect>(inherit: false);
            
            var allAspects = declaredTypeAspects
                .Union(methodAspects)
                .Where(a => !a.GetType().IsAbstract)
                .Distinct()
                .OrderBy(a => a.Order)
                .ToList();

            for (var i = 0; i < allAspects.Count; i++)
            {
                allAspects[i].Order = allAspects[i].Order + i;
                allAspects[i].HasDependencies = MethodAspectHasDependencies(allAspects[i]);
            }

            return allAspects;
        }

        /// <summary>
        /// Создает аспект перехвата вызова метода.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Аспект перехвата вызова метода.</returns>
        public MethodInterceptionAspect CreateMethodInterceptionAspect(InvocationContext context)
        {
            var methodAspects = context.Method.GetCustomAttributes<MethodInterceptionAspect>(inherit: false).ToList();
            if (methodAspects.Count > 1)
            {
                throw new IvorySharpException(
                    $"Допустимо наличие только одного аспекта типа '{typeof(MethodInterceptionAspect)}'. " +
                    $"На методе '{context.Method.Name}' типа '{context.InstanceDeclaringType.FullName}' задано несколько.");
            }

            if (methodAspects.Count == 0)
                return NullMethodInterceptionAspect.Instance;

            var methodAspect = methodAspects.Single();
            
            methodAspect.HasDependencies = MethodAspectHasDependencies(methodAspect);

            return methodAspect;
        }
        
        private bool MethodAspectHasDependencies(MethodAspect aspect)
        {
            return aspect.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Any(p => p.CanWrite && p.GetCustomAttribute<InjectDependencyAttribute>() != null);
        }
    }
}