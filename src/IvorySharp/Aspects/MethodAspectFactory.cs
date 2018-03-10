using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Core;

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
        /// <param name="context"></param>
        /// <returns>Коллекция аспектов.</returns>
        public MethodBoundaryAspect[] CreateMethodBoundaryAspects(InvocationContext context)
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
                .ToArray();

            for (var i = 0; i < allAspects.Length; i++)
            {
                allAspects[i].Order = allAspects[i].Order + i;
                allAspects[i].HasDependencies = MethodAspectHasDependencies(allAspects[i]);
            }

            return allAspects;
        }

        private bool MethodAspectHasDependencies(MethodAspect aspect)
        {
            return aspect.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Any(p => p.CanWrite && p.GetCustomAttribute<InjectDependencyAttribute>() != null);
        }
    }
}