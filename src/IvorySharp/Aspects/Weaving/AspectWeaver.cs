using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    [PublicAPI]
    public sealed class AspectWeaver
    {
        private readonly IComponentProvider<IInvocationPipelineFactory> _pipelineFactoryProvider;
        private readonly IComponentProvider<IAspectFactory<MethodBoundaryAspect>> _boundaryAspectFactoryProvider;
        private readonly IComponentProvider<IAspectFactory<MethodInterceptionAspect>> _interceptionAspectFactoryProvider;
        private readonly IComponentProvider<IAspectWeavePredicate> _aspectWeavePredicateProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        public AspectWeaver(
            IComponentProvider<IAspectWeavePredicate> aspectWeavePredicateProvider, 
            IComponentProvider<IAspectFactory<MethodBoundaryAspect>> boundaryAspectFactoryProvider,
            IComponentProvider<IAspectFactory<MethodInterceptionAspect>> interceptionAspectFactoryProvider,
            IComponentProvider<IInvocationPipelineFactory> pipelineFactoryProvider)
        {
            _pipelineFactoryProvider = pipelineFactoryProvider;
            _interceptionAspectFactoryProvider = interceptionAspectFactoryProvider;
            _boundaryAspectFactoryProvider = boundaryAspectFactoryProvider;
            _aspectWeavePredicateProvider = aspectWeavePredicateProvider;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="declaringType">Объявленный тип исходного объекта.</param>
        /// <param name="targetType">Фактический тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="declaringType"/>.</returns>
        public object Weave(object target, Type declaringType, Type targetType)
        {
            return AspectWeaveProxy.Create(
                target, 
                targetType, 
                declaringType, 
                _boundaryAspectFactoryProvider,
                _interceptionAspectFactoryProvider,
                _pipelineFactoryProvider,
                _aspectWeavePredicateProvider);
        }
    }
}