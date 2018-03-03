using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Селектор аспектов.
    /// </summary>
    public interface IMethodAspectSelector
    {
        IReadOnlyCollection<IMethodBoundaryAspect> GetMethodBoundaryAspects(IInvocation invocation);
    }

    public class MethodAspectSelector : IMethodAspectSelector
    {
        private static ConcurrentDictionary<MethodInfo, AspectBinding[]> _aspectsCache;

        static MethodAspectSelector()
        {
            _aspectsCache = new ConcurrentDictionary<MethodInfo, AspectBinding[]>();
        }
            
        public IReadOnlyCollection<IMethodBoundaryAspect> GetMethodBoundaryAspects(IInvocation invocation)
        {
            var aspectBindings = GetAspectBindings(invocation);
            var aspects = Array.ConvertAll(aspectBindings, b => b.Creator());
            return aspects;
        }

        private AspectBinding[] GetAspectBindings(IInvocation invocation)
        {
            if (_aspectsCache.TryGetValue(invocation.Context.Method, out var cached))
                return cached;

            var aspectTypes = invocation.Context.Method
                .GetCustomAttributes<MethodBoundaryAspect>(inherit: false)
                .Select(a => a.GetType())
                .Where(t => !t.IsAbstract)
                .Select(t => new AspectBinding(t, Expressions.CreateFactoryMethod<IMethodBoundaryAspect>(t)))
                .ToArray();

            _aspectsCache.AddOrUpdate(invocation.Context.Method, _ => aspectTypes, (_, __) => aspectTypes);

            return aspectTypes;
        }
        
        public struct AspectBinding
        {
            public Func<IMethodBoundaryAspect> Creator;
            public Type AspectType;

            public AspectBinding(Type aspectType, Func<IMethodBoundaryAspect> creator)
            {
                AspectType = aspectType;
                Creator = creator;
            }
        }
    }
}