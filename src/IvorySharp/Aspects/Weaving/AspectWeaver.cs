using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class AspectWeaver
    {
        private readonly IComponentHolder<IInvocationPipelineFactory> _pipelineFactoryHolder;
        private readonly IComponentHolder<IAspectFactory> _aspectFactoryHolder;
        private readonly IComponentHolder<IAspectWeavePredicate> _aspectWeavePredicateHolder;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        public AspectWeaver(
            IComponentHolder<IAspectWeavePredicate> aspectWeavePredicateHolder, 
            IComponentHolder<IInvocationPipelineFactory> pipelineFactoryHolder, 
            IComponentHolder<IAspectFactory> aspectFactoryHolder)
        {
            _pipelineFactoryHolder = pipelineFactoryHolder;
            _aspectFactoryHolder = aspectFactoryHolder;
            _aspectWeavePredicateHolder = aspectWeavePredicateHolder;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="declaringType">Объявленный тип исходного объекта.</param>
        /// <param name="targetType">Фактический тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="declaringType"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Weave(object target, Type declaringType, Type targetType)
        {
            return AspectWeaveProxy.Create(
                target, 
                targetType, 
                declaringType, 
                _aspectFactoryHolder, 
                _pipelineFactoryHolder,
                _aspectWeavePredicateHolder);
        }
    }
}