using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент для предварительной инициализации аспектов.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспектов.</typeparam>
    internal class DefaultAspectPreInitializer<TAspect> : IAspectPreInitializer<TAspect>
        where TAspect : OrderableMethodAspect
    {
        private readonly IComponentProvider<IAspectDeclarationCollector> _aspectDeclarationCollectorProvider;
        private readonly IComponentProvider<IAspectOrderStrategy> _orderStrategyProvider;

        public DefaultAspectPreInitializer(
            IComponentProvider<IAspectDeclarationCollector> aspectDeclarationCollectorProvider,
            IComponentProvider<IAspectOrderStrategy> orderStrategyProvider)
        {
            _aspectDeclarationCollectorProvider = aspectDeclarationCollectorProvider;
            _orderStrategyProvider = orderStrategyProvider;
        }

        /// <inheritdoc />
        public TAspect[] PrepareAspects(IInvocationContext context) 
        {
            var collector = _aspectDeclarationCollectorProvider.Get();
            var orderer = _orderStrategyProvider.Get();
            
            var methodBoundaryAspects = new List<TAspect>();
            var declarations = collector.CollectAspectDeclarations<TAspect>(context);

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
    }
}