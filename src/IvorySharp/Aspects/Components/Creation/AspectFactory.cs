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
    internal class AspectFactory : IAspectFactory
    {
        private readonly IAspectDeclarationCollector _aspectDeclarationCollector;
        private readonly IAspectDependencyInjector _aspectDependencyInjector;
        private readonly IAspectOrderStrategy _aspectOrderStrategy;

        private readonly Func<InvocationContext, MethodBoundaryAspect[]> _cachedPrepareMethodBoundaryAspect;
        private readonly Func<InvocationContext, MethodInterceptionAspect> _cachedPrepareMethodInterceptionAspect;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectFactory"/>.
        /// </summary>
        public AspectFactory(
            IAspectDeclarationCollector aspectDeclarationCollector,
            IAspectDependencyInjector aspectDependencyInjector, 
            IAspectOrderStrategy aspectOrderStrategy)
        {
            _aspectDeclarationCollector = aspectDeclarationCollector;
            _aspectDependencyInjector = aspectDependencyInjector;
            _aspectOrderStrategy = aspectOrderStrategy;

            _cachedPrepareMethodBoundaryAspect = Cache.CreateProducer(PrepareBoundaryAspects, InvocationContext.ByMethodEqualityComparer.Instance);
            _cachedPrepareMethodInterceptionAspect = Cache.CreateProducer(PrepareInterceptAspect, InvocationContext.ByMethodEqualityComparer.Instance);
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(InvocationContext context)
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
        public MethodInterceptionAspect CreateInterceptionAspect(InvocationContext context)
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

            var orderedAspects = _aspectOrderStrategy.Order(aspectDeclarations.Select(a => a.MethodAspect)).ToArray();
            
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
                return BypassMethodAspect.Instance;

            var declaration = aspectDeclarations.Single();

            declaration.MethodAspect.JoinPointType = declaration.JoinPointType;

            return declaration.MethodAspect;
        }
    }
}