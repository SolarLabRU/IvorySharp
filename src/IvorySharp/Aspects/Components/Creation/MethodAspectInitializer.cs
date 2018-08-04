using System;
using System.Linq;
using IvorySharp.Aspects.Components.Caching;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Core;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects.Components.Creation
{
    /// <summary>
    /// Инициализатор аспектов.
    /// </summary>
    internal class MethodAspectInitializer : IMethodAspectInitializer
    {
        private readonly IMethodAspectDeclarationCollector _aspectDeclarationCollector;
        private readonly IMethodAspectDependencyInjector _aspectDependencyInjector;
        private readonly IMethodAspectOrderStrategy _methodAspectOrderStrategy;

        private readonly Func<InvocationContext, MethodBoundaryAspect[]> _cachedPrepareMethodBoundaryAspect;
        private readonly Func<InvocationContext, MethodInterceptionAspect> _cachedPrepareMethodInterceptionAspect;

        public MethodAspectInitializer(
            IMethodAspectDeclarationCollector aspectDeclarationCollector,
            IMethodAspectDependencyInjector aspectDependencyInjector, 
            IMethodAspectOrderStrategy methodAspectOrderStrategy)
        {
            _aspectDeclarationCollector = aspectDeclarationCollector;
            _aspectDependencyInjector = aspectDependencyInjector;
            _methodAspectOrderStrategy = methodAspectOrderStrategy;

            _cachedPrepareMethodBoundaryAspect = Cache.CreateProducer(PrepareBoundaryAspects, InvocationContext.ByMethodEqualityComparer.Instance);
            _cachedPrepareMethodInterceptionAspect = Cache.CreateProducer(PrepareInterceptAspect, InvocationContext.ByMethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] InitializeBoundaryAspects(InvocationContext context)
        {
            var aspects = _cachedPrepareMethodBoundaryAspect(context);

            foreach (var aspect in aspects)
            {
                _aspectDependencyInjector.InjectPropertyDependencies(aspect);
                aspect.Initialize();
            }

            return aspects;
        }

        /// <inheritdoc />
        public MethodInterceptionAspect InitializeInterceptionAspect(InvocationContext context)
        {
            var aspect = _cachedPrepareMethodInterceptionAspect(context);

            _aspectDependencyInjector.InjectPropertyDependencies(aspect);
            aspect.Initialize();

            return aspect;
        }

        internal MethodBoundaryAspect[] PrepareBoundaryAspects(InvocationContext context)
        {
            var aspectDeclarations = _aspectDeclarationCollector.CollectAspectDeclarations<MethodBoundaryAspect>(context);

            foreach (var declaration in aspectDeclarations)
            {
                declaration.MethodAspect.JoinPointType = declaration.JoinPointType;
            }

            var orderedAspects = _methodAspectOrderStrategy
                .Order(aspectDeclarations
                .Select(a => a.MethodAspect))
                .ToArray();
            
            for (var i = 0; i < orderedAspects.Length; i++)
            {
                aspectDeclarations[i].MethodAspect.InternalOrder = aspectDeclarations[i].MethodAspect.Order + i + 1;
            }

            return orderedAspects;
        }

        internal MethodInterceptionAspect PrepareInterceptAspect(InvocationContext context)
        {
            var aspectDeclarations = _aspectDeclarationCollector.CollectAspectDeclarations<MethodInterceptionAspect>(context);

            if (aspectDeclarations.Length > 1)
            {
                throw new IvorySharpException(
                    $"Допустимо наличие только одного аспекта типа '{typeof(MethodInterceptionAspect)}'. " +
                    $"На методе '{context.Method.Name}' типа '{context.DeclaringType.FullName}' задано несколько.");
            }

            if (aspectDeclarations.Length == 0)
                return NullMethodInterceptionAspect.Instance;

            var declaration = aspectDeclarations.Single();

            declaration.MethodAspect.JoinPointType = declaration.JoinPointType;

            return declaration.MethodAspect;
        }
    }
}