using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент подготовки аспектов для инициализации.
    /// </summary>
    internal sealed class DefaultAspectsPreInitializer : IAspectsPreInitializer
    {
        private readonly IComponentProvider<IAspectDeclarationCollector> _aspectDeclarationCollectorProvider;
        private readonly IComponentProvider<IAspectOrderStrategy> _orderStrategyProvider;  

        /// <summary>
        /// Инициализирует экземпляр <see cref="DefaultAspectsPreInitializer"/>.
        /// </summary>
        public DefaultAspectsPreInitializer(
            IComponentProvider<IAspectDeclarationCollector> aspectDeclarationCollectorProvider,
            IComponentProvider<IAspectOrderStrategy> orderStrategyProvider)
        {
            _aspectDeclarationCollectorProvider = aspectDeclarationCollectorProvider;
            _orderStrategyProvider = orderStrategyProvider;
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] PrepareBoundaryAspects(IInvocationContext context)
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

        /// <inheritdoc />
        public MethodInterceptionAspect PrepareInterceptAspect(IInvocationContext context)
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