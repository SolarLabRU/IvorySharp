using System;
using IvorySharp.Aspects.Components.Caching;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Helpers;

namespace IvorySharp.Aspects.Components.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    public class AspectWeaver
    {
        private readonly IPipelineExecutor _aspectPipelineExecutor;
        private readonly IAspectFactory _aspectFactory;
        private readonly IAspectWeavePredicate _aspectWeavePredicate;

        private readonly Func<TypePair, bool> _cachedWeaveable;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        public AspectWeaver(
            IAspectWeavePredicate aspectWeavePredicate, 
            IPipelineExecutor aspectPipelineExecutor, 
            IAspectFactory aspectFactory)
        {
            _cachedWeaveable = Cache.CreateProducer(tp => aspectWeavePredicate
                    .IsWeaveable(tp.DeclaringType, tp.TargetType),
                TypePair.EqualityComparer.Instance);

            _aspectPipelineExecutor = aspectPipelineExecutor;
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
            if (!_cachedWeaveable(new TypePair(declaringType, targetType)))
                return target;

            return AspectWeavedProxy.Create(target, targetType, declaringType, _aspectFactory, _aspectPipelineExecutor, _aspectWeavePredicate);
        }
    }
}