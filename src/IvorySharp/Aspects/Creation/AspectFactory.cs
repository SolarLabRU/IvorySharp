using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Components;
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
    internal sealed class AspectFactory : IAspectFactory
    {
        private readonly IComponentProvider<IAspectDeclarationCollector> _aspectDeclarationCollectorProvider;
        private readonly IComponentProvider<IAspectDependencyInjector> _dependencyInjectorProvider;
        private readonly IComponentProvider<IAspectOrderStrategy> _orderStrategyProvider;
        
        private readonly Func<IInvocationContext, MethodBoundaryAspect[]> _cachedPrepareMethodBoundaryAspect;
        private readonly Func<IInvocationContext, MethodInterceptionAspect> _cachedPrepareMethodInterceptionAspect;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectFactory"/>.
        /// </summary>
        public AspectFactory(
            IComponentProvider<IAspectDeclarationCollector> aspectDeclarationCollectorProvider,
            IComponentProvider<IAspectDependencyInjector> dependencyInjectorProvider,
            IComponentProvider<IAspectOrderStrategy> orderStrategyProvider)
        {
            _aspectDeclarationCollectorProvider = aspectDeclarationCollectorProvider;
            _dependencyInjectorProvider = dependencyInjectorProvider;
            _orderStrategyProvider = orderStrategyProvider;
            
            _cachedPrepareMethodBoundaryAspect = Memoizer.CreateProducer(PrepareBoundaryAspects,
                InvocationContextMethodComparer.Instance);
            
            _cachedPrepareMethodInterceptionAspect = Memoizer.CreateProducer(PrepareInterceptAspect,
                InvocationContextMethodComparer.Instance);
        }  

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationContext context)
        {
            var aspects = _cachedPrepareMethodBoundaryAspect(context);
            var dependencyInjector = _dependencyInjectorProvider.Get();
            
            foreach (var aspect in aspects)
            {
                dependencyInjector.InjectPropertyDependencies(aspect);
                aspect.Initialize();
            }

            return aspects;
        }

        /// <inheritdoc />
        public MethodInterceptionAspect CreateInterceptionAspect(IInvocationContext context)
        {
            var aspect = _cachedPrepareMethodInterceptionAspect(context);
            var dependencyInjector = _dependencyInjectorProvider.Get();
            
            dependencyInjector.InjectPropertyDependencies(aspect);
            aspect.Initialize();

            return aspect;
        }

        /// <summary>
        /// Подготавливает аспекты типа <see cref="MethodBoundaryAspect"/> для инициализации.
        /// </summary>
        /// <param name="context">Контекст вызова метода.</param>
        /// <returns>Массив не инициализированных аспектов.</returns>
        internal MethodBoundaryAspect[] PrepareBoundaryAspects(IInvocationContext context)
        {
            var collector = _aspectDeclarationCollectorProvider.Get();
            var orderer = _orderStrategyProvider.Get();
            
            var methodBoundaryAspects = new List<MethodBoundaryAspect>();
            var declarations = collector.CollectAspectDeclarations<MethodBoundaryAspect>(context);

            foreach (var aspect in orderer.Order(declarations.Select(d => d.MethodAspect)))
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

        /// <summary>
        /// Подготавливает аспект типа <see cref="MethodInterceptionAspect"/> для инициализации.
        /// </summary>
        /// <param name="context">Контекст вызова метода.</param>
        /// <returns>Не инициализированный аспект типа <see cref="MethodInterceptionAspect"/>.</returns>
        internal MethodInterceptionAspect PrepareInterceptAspect(IInvocationContext context)
        {
            var collector = _aspectDeclarationCollectorProvider.Get();
            
            var aspectDeclarations = collector
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
            declaration.MethodAspect.InternalId = Guid.NewGuid();

            return declaration.MethodAspect;
        }
    }
}