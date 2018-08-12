using System;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    [PublicAPI]
    public class AspectWeaver
    {
        private readonly IInvocationPipelineFactory _pipelineFactory;
        private readonly IAspectFactory _aspectFactory;
        private readonly IAspectWeavePredicate _aspectWeavePredicate;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        public AspectWeaver(
            IAspectWeavePredicate aspectWeavePredicate, 
            IInvocationPipelineFactory pipelineFactory, 
            IAspectFactory aspectFactory)
        {
            _pipelineFactory = pipelineFactory;
            _aspectFactory = aspectFactory;
            _aspectWeavePredicate = aspectWeavePredicate;
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
                _aspectFactory, 
                _pipelineFactory,
                _aspectWeavePredicate);
        }
    }
}