using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Core;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Реализация селектора аспектов.
    /// </summary>
    internal class MethodAspectSelector
    {
        private static readonly ConcurrentDictionary<MethodInfo, List<MethodBoundaryAspect>> _cache;

        /// <summary>
        /// Экземпляр селектора аспектов.
        /// </summary>
        internal static MethodAspectSelector Instance { get; } = new MethodAspectSelector();

        static MethodAspectSelector()
        {
            _cache = new ConcurrentDictionary<MethodInfo, List<MethodBoundaryAspect>>();
        }

        private MethodAspectSelector()
        {
        }

        /// <summary>
        /// Получает все допустимые к применению аспекты для вызова.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Коллекция аспектов.</returns>
        public MethodBoundaryAspect[] GetMethodBoundaryAspects(InvocationContext context)
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
                allAspects[i].Order = allAspects[i].Order + i;

            return allAspects;
        }
    }
}