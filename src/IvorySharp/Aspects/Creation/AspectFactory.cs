using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Aspects.Selection;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент подготовки аспектов для инициализации.
    /// </summary>
    internal sealed class AspectFactory : IAspectFactory
    {
        private readonly IComponentHolder<IAspectDeclarationCollector> _aspectDeclarationCollectorHolder;
        private readonly IComponentHolder<IAspectOrderStrategy> _orderStrategyHolder;
        private readonly IComponentHolder<IAspectDependencySelector> _dependencySelectorHolder;
        private readonly IComponentHolder<IAspectFinalizer> _aspectFinalizerHolder;
        
        private IAspectDeclarationCollector _aspectDeclarationCollector;
        private IAspectOrderStrategy _aspectOrderStrategy;
        private IAspectDependencySelector _dependencySelector;
        private IAspectFinalizer _aspectFinalizer;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectFactory"/>.
        /// </summary>
        public AspectFactory(
            IComponentHolder<IAspectDeclarationCollector> aspectDeclarationCollectorHolder,
            IComponentHolder<IAspectOrderStrategy> orderStrategyHolder,
            IComponentHolder<IAspectDependencySelector> dependencySelectorHolder,
            IComponentHolder<IAspectFinalizer> aspectFinalizerHolder)
        {
            _aspectDeclarationCollectorHolder = aspectDeclarationCollectorHolder;
            _orderStrategyHolder = orderStrategyHolder;
            _dependencySelectorHolder = dependencySelectorHolder;
            _aspectFinalizerHolder = aspectFinalizerHolder;
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] CreateBoundaryAspects(IInvocationSignature signature)
        {
            if (_aspectDeclarationCollector == null)
                _aspectDeclarationCollector = _aspectDeclarationCollectorHolder.Get();

            if (_aspectOrderStrategy == null)
                _aspectOrderStrategy = _orderStrategyHolder.Get();

            if (_aspectFinalizer == null)
                _aspectFinalizer = _aspectFinalizerHolder.Get();
            
            if (_dependencySelector == null)
                _dependencySelector = _dependencySelectorHolder.Get();
            
            var methodBoundaryAspects = new List<MethodBoundaryAspect>();
            var declarations = _aspectDeclarationCollector.CollectAspectDeclarations<MethodBoundaryAspect>(signature);

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
                currentAspect.HasDependencies = _dependencySelector.HasDependencies(currentAspect.GetType());
                currentAspect.IsFinalizable = _aspectFinalizer.IsFinalizable(currentAspect);
                currentAspect.IsInitializable = ReflectedMethod.IsOverriden(
                    MethodAspect.GetInitializeMethod(currentAspect));
            }
            
            return methodBoundaryAspects.ToArray();
        }

        /// <inheritdoc />
        public MethodInterceptionAspect CreateInterceptAspect(IInvocationSignature context)
        {
            if (_aspectDeclarationCollector == null)
                _aspectDeclarationCollector = _aspectDeclarationCollectorHolder.Get();
       
            if (_aspectFinalizer == null)
                _aspectFinalizer = _aspectFinalizerHolder.Get();
            
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
     
            if (_dependencySelector == null)
                _dependencySelector = _dependencySelectorHolder.Get();
            
            var declaration = aspectDeclarations.Single();

            declaration.MethodAspect.MulticastTarget = declaration.MulticastTarget;
            declaration.MethodAspect.InternalId = Guid.NewGuid();
            declaration.MethodAspect.HasDependencies = _dependencySelector.HasDependencies(declaration.MethodAspect.GetType());
            declaration.MethodAspect.IsFinalizable = _aspectFinalizer.IsFinalizable(declaration.MethodAspect);
            declaration.MethodAspect.IsInitializable = ReflectedMethod.IsOverriden(
                MethodAspect.GetInitializeMethod(declaration.MethodAspect));
            
            return declaration.MethodAspect;
        }
    }
}