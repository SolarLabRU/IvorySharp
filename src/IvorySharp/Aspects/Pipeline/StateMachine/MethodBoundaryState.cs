using System.Collections.Generic;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Базовая модель состояния выполнения пайплайна, характеризующаяся вызовом
    /// точки прикрепления аспектов типа <see cref="MethodBoundaryAspect"/>.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal abstract class MethodBoundaryState<TPipeline> : InvocationState<TPipeline> where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Перечень аспектов для применения.
        /// </summary>
        internal IEnumerable<MethodBoundaryAspect> BoundaryAspects { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodBoundaryState{TPipeline}"/>.
        /// </summary>
        /// <param name="boundaryAspects">Перечень аспектов для применения.</param>
        protected MethodBoundaryState(IEnumerable<MethodBoundaryAspect> boundaryAspects)
        {
            BoundaryAspects = boundaryAspects;
        }

        /// <inheritdoc />
        internal override InvocationState<TPipeline> MakeTransition(TPipeline pipeline)
        {
            foreach (var aspect in BoundaryAspects)
            {
                pipeline.ExecutionStateKey = aspect.InternalId;

                Apply(aspect, pipeline);

                if (ShouldBreak(new BoundaryStateData(pipeline, aspect), out var transition))
                {
                    pipeline.FlowBehavior = FlowBehavior.Continue;
                    return transition;
                }
            }

            return CreateContinuation();
        }

        /// <summary>
        /// Выполняет применение аспекта <paramref name="aspect"/> на пайплайн <paramref name="pipeline"/>.
        /// </summary>
        /// <param name="aspect">Аспект.</param>
        /// <param name="pipeline">Пайплайн.</param>
        protected abstract void Apply(MethodBoundaryAspect aspect, TPipeline pipeline);

        /// <summary>
        /// Возвращает признак того, что необходимо прервать выполнение текущего действия и
        /// выполнить переход в состояние <paramref name="transition"/>.
        /// </summary>
        /// <param name="data">Контекст выполнения состояния.</param>
        /// <param name="transition">Состояние, в которое необходимо выполнить переход.</param>
        /// <returns>Признак того, что необходимо прервать выполнение текущего действия.</returns>
        protected abstract bool ShouldBreak(BoundaryStateData data, out InvocationState<TPipeline> transition);

        /// <summary>
        /// Создает переход к следующему состоянию (при успешном выполнении действия).
        /// </summary>
        /// <returns>Результирующий переход.</returns>
        protected abstract InvocationState<TPipeline> CreateContinuation();
        
        /// <summary>
        /// Контекст выполнения действия.
        /// </summary>
        internal struct BoundaryStateData
        {
            /// <summary>
            /// Пайплайн выполнения метода.
            /// </summary>
            public readonly TPipeline Pipeline;
            
            /// <summary>
            /// Текущий выполняющийся аспект.
            /// </summary>
            public readonly MethodBoundaryAspect CurrentAspect;

            /// <summary>
            /// Инициализирует экземпляр <see cref="BoundaryStateData"/>.
            /// </summary>
            public BoundaryStateData(TPipeline pipeline, MethodBoundaryAspect currentAspect)
            {
                Pipeline = pipeline;
                CurrentAspect = currentAspect;
            }
        }
    }
}