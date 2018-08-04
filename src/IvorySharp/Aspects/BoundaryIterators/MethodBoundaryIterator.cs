using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Aspects.BoundaryIterators
{
    /// <summary>
    /// Базовый класс итератора аспектов типа <see cref="MethodBoundaryAspect"/>.
    /// </summary>
    internal abstract class MethodBoundaryIterator
    {
        /// <summary>
        /// Модель пайплайна вызова.
        /// </summary>
        public InvocationPipeline Pipeline { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodBoundaryIterator"/>.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна вызова.</param>
        protected MethodBoundaryIterator(InvocationPipeline pipeline)
        {
            Pipeline = pipeline;
        }

        /// <summary>
        /// Выполняет обход точек прикрепления коллекции аспектов.
        /// </summary>
        /// <param name="aspects">Коллекция аспектов.</param>
        /// <param name="orderInclusiveWall">Барьер приоритета аспекта. Если задан, то все аспекты с приоритетом выше выполнены не будут.</param>
        /// <returns>Результат обхода.</returns>
        internal BoundaryIterationResult Iterate(
            IReadOnlyCollection<MethodBoundaryAspect> aspects,
            int? orderInclusiveWall = null)
        {
            for (var i = 0; i < aspects.Count; i++)
            {
                var aspect = aspects.ElementAt(i);

                try
                {
                    Pipeline.CurrentExecutingAspect = aspect;

                    if (CanContinue(Pipeline.FlowBehavior))
                    {
                        if (orderInclusiveWall.HasValue && aspect.InternalOrder >= orderInclusiveWall.Value)
                            continue;

                        ExecuteAspect(aspect, Pipeline);
                        if (ShouldBreak(Pipeline.FlowBehavior))
                        {
                            return BoundaryIterationResult.BreakedBy(aspect);
                        }
                    }
                }
                catch (Exception e)
                {
                    Pipeline.ThrowException(e);
                    return BoundaryIterationResult.BreakedBy(aspect);
                }
            }

            return BoundaryIterationResult.Succeed();
        }

        /// <summary>
        /// Возвращает признак возможности продолжения обхода аспектов.
        /// </summary>
        /// <param name="flowBehavior">Текущее состояние пайплайна.</param>
        /// <returns>Признак возможности продолжения обхода аспектов.</returns>
        protected abstract bool CanContinue(FlowBehavior flowBehavior);

        /// <summary>
        /// Возвращает признак необходимости прервать обход аспектов.
        /// </summary>
        /// <param name="flowBehavior">Текущее состояние пайплайна.</param>
        /// <returns>Признак необходимости прервать обход аспектов.</returns>
        protected abstract bool ShouldBreak(FlowBehavior flowBehavior);

        /// <summary>
        /// Выполняет точку прикрепления аспекта.
        /// </summary>
        /// <param name="aspect">Модель аспекта.</param>
        /// <param name="pipeline">Пайплайн вызова.</param>
        protected abstract void ExecuteAspect(MethodBoundaryAspect aspect, IInvocationPipeline pipeline);

        /// <summary>
        /// Результат итерации аспектов.
        /// </summary>
        internal struct BoundaryIterationResult
        {
            public readonly MethodBoundaryAspect Breaker;

            /// <summary>
            /// Инициализирует экземпляр <see cref="BoundaryIterationResult"/>.
            /// </summary>
            /// <param name="breaker">Аспект, который прервал выполнение метода.</param>
            private BoundaryIterationResult(MethodBoundaryAspect breaker)
            {
                Breaker = breaker;
            }

            /// <summary>
            /// Создает успешный результат обхода.
            /// </summary>
            /// <returns>Результат обхода.</returns>
            public static BoundaryIterationResult Succeed()
            {
                return new BoundaryIterationResult(null);
            }

            /// <summary>
            /// Создает неудачный результат обхода.
            /// </summary>
            /// <param name="breaker">Аспект, который прервал выполнение метода.</param>
            /// <returns>Результат обхода.</returns>
            public static BoundaryIterationResult BreakedBy(MethodBoundaryAspect breaker)
            {
                return new BoundaryIterationResult(breaker);
            }
        }
    }
}