using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using DependencyAttribute = IvorySharp.Aspects.Dependency.DependencyAttribute;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент подготовки аспектов для инициализации.
    /// </summary>
    internal sealed class DefaultAspectsPreInitializer : IAspectsPreInitializer
    {
        private readonly IComponentHolder<IAspectDeclarationCollector> _aspectDeclarationCollectorHolder;
        private readonly IComponentHolder<IAspectOrderStrategy> _orderStrategyHolder;

        private IAspectDeclarationCollector _aspectDeclarationCollector;
        private IAspectOrderStrategy _aspectOrderStrategy;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="DefaultAspectsPreInitializer"/>.
        /// </summary>
        public DefaultAspectsPreInitializer(
            IComponentHolder<IAspectDeclarationCollector> aspectDeclarationCollectorHolder,
            IComponentHolder<IAspectOrderStrategy> orderStrategyHolder)
        {
            _aspectDeclarationCollectorHolder = aspectDeclarationCollectorHolder;
            _orderStrategyHolder = orderStrategyHolder;
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] PrepareBoundaryAspects(IInvocationContext context)
        {
            if (_aspectDeclarationCollector == null)
                _aspectDeclarationCollector = _aspectDeclarationCollectorHolder.Get();

            if (_aspectOrderStrategy == null)
                _aspectOrderStrategy = _orderStrategyHolder.Get();
            
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
                var currentAspect = methodBoundaryAspects[i];
                
                currentAspect.InternalOrder = currentAspect.Order + i + 1;
                currentAspect.InternalId = Guid.NewGuid();
                currentAspect.HasDependencies = HasDependencies(currentAspect.GetType());
            }
            
            return methodBoundaryAspects.ToArray();
        }

        /// <inheritdoc />
        public MethodInterceptionAspect PrepareInterceptAspect(IInvocationContext context)
        {
            if (_aspectDeclarationCollector == null)
                _aspectDeclarationCollector = _aspectDeclarationCollectorHolder.Get();
       
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
            declaration.MethodAspect.InternalId = Guid.NewGuid();
            declaration.MethodAspect.HasDependencies = HasDependencies(declaration.MethodAspect.GetType());

            return declaration.MethodAspect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasDependencies(Type aspectType)
        {
            return aspectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.CanWrite &&
                          p.CustomAttributes.Select(ca => ca.AttributeType)
                              .Any(t => t == typeof(DependencyAttribute)));
        }
    }
}