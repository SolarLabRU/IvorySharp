using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Selection;
using IvorySharp.Caching;
using IvorySharp.Core;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Инициализатор аспектов.
    /// </summary>
    internal class AspectFactory : IAspectFactory
    {
        private readonly IAspectDeclarationCollector _aspectDeclarationCollector;
        private readonly IAspectDependencyInjector _aspectDependencyInjector;
        private readonly IAspectOrderStrategy _aspectOrderStrategy;

        private readonly Func<IInvocationContext, MethodBoundaryAspect[]> _cachedPrepareMethodBoundaryAspect;
        private readonly Func<IInvocationContext, MethodInterceptionAspect> _cachedPrepareMethodInterceptionAspect;

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

            _cachedPrepareMethodBoundaryAspect = Memoizer.CreateProducer(PrepareBoundaryAspects,
                InvocationContextMethodComparer.Instance);
            
            _cachedPrepareMethodInterceptionAspect = Memoizer.CreateProducer(PrepareInterceptAspect,
                InvocationContextMethodComparer.Instance);
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationContext context)
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
        public MethodInterceptionAspect CreateInterceptionAspect(IInvocationContext context)
        {
            var aspect = _cachedPrepareMethodInterceptionAspect(context);

            _aspectDependencyInjector.InjectPropertyDependencies(aspect);
            aspect.Initialize();

            return aspect;
        }

        internal MethodBoundaryAspect[] PrepareBoundaryAspects(IInvocationContext context)
        {
            var methodBoundaryAspects = new List<MethodBoundaryAspect>();
            var declarations = _aspectDeclarationCollector.CollectAspectDeclarations<MethodBoundaryAspect>(context);

            foreach (var aspect in _aspectOrderStrategy.Order(declarations.Select(d => d.MethodAspect)))
            {
                var existingAspect = methodBoundaryAspects.Find(aspect.Equals);
                
                // Если у текущего аспекта приоритет выше, чем равного тому,
                // что уже есть в коллекции, то заменяем его на новый
                if (existingAspect != null && aspect.Order < existingAspect.Order)
                    methodBoundaryAspects.Remove(existingAspect);
                else if (existingAspect == null)
                    methodBoundaryAspects.Add(aspect);                
            }

            for (var i = 0; i < methodBoundaryAspects.Count; i++)
            {
                methodBoundaryAspects[i].InternalOrder = methodBoundaryAspects[i].Order + i + 1;
                methodBoundaryAspects[i].InternalId = Guid.NewGuid();
            }
            
            return methodBoundaryAspects.ToArray();
        }

        internal MethodInterceptionAspect PrepareInterceptAspect(IInvocationContext context)
        {
            var aspectDeclarations = _aspectDeclarationCollector
                .CollectAspectDeclarations<MethodInterceptionAspect>(context)
                .ToArray();

            if (aspectDeclarations.Length > 1)
            {
                throw new IvorySharpException(
                    $"Допустимо наличие только одного аспекта типа '{typeof(MethodInterceptionAspect)}'. " +
                    $"На методе '{context.Method.Name}' типа '{context.DeclaringType.FullName}' задано несколько.");
            }

            if (aspectDeclarations.Length == 0)
                return BypassMethodAspect.Instance;

            var declaration = aspectDeclarations.Single();

            declaration.MethodAspect.MulticastTarget = declaration.MulticastTarget;

            return declaration.MethodAspect;
        }
    }
}