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
        private static ConcurrentDictionary<MethodInfo, List<MethodBoundaryAspect>> _cache;

        /// <summary>
        /// Экземпляр селектора аспектов.
        /// </summary>
        internal static MethodAspectSelector Instance { get; } = new MethodAspectSelector();

        static MethodAspectSelector()
        {
            _cache = new ConcurrentDictionary<MethodInfo, List<MethodBoundaryAspect>>();
        }
        
        private MethodAspectSelector() { }

        /// <summary>
        /// Получает все допустимые к применению аспекты для вызова.
        /// </summary>
        /// <param name="invocation">Модель вызова.</param>
        /// <returns>Коллекция аспектов.</returns>
        public IReadOnlyCollection<IMethodBoundaryAspect> GetMethodBoundaryAspects(IInvocation invocation)
        {
            if (!_cache.TryGetValue(invocation.Context.Method, out var aspects))
            {
                var declaredTypeAspects = invocation.Context.InstanceDeclaringType
                    .GetCustomAttributes<MethodBoundaryAspect>();

                var methodAspects = invocation.Context.Method
                    .GetCustomAttributes<MethodBoundaryAspect>();

                var allAspects = declaredTypeAspects
                    .Union(methodAspects)
                    .Where(a => !a.GetType().IsAbstract)
                    .Distinct()
                    .OrderBy(a => a.Order)
                    .ToList();

                _cache[invocation.Context.Method] = allAspects;
                
                return allAspects;
            }

            return aspects;
        }
    }
}