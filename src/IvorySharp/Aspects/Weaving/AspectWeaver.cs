using System;
using IvorySharp.Aspects.Components;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
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
        private readonly IComponentProvider<IAspectFactory> _aspectFactoryProvider;
        private readonly IComponentProvider<IAspectWeavePredicate> _aspectWeavePredicateProvider;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        public AspectWeaver(
            IComponentProvider<IAspectWeavePredicate> aspectWeavePredicateProvider, 
            IComponentProvider<IInvocationPipelineFactory> pipelineFactoryProvider, 
            IComponentProvider<IAspectFactory> aspectFactoryProvider)
        {
            _pipelineFactoryProvider = pipelineFactoryProvider;
            _aspectFactoryProvider = aspectFactoryProvider;
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
                _aspectFactoryProvider, 
                _pipelineFactoryProvider,
                _aspectWeavePredicateProvider);
        }
    }
}