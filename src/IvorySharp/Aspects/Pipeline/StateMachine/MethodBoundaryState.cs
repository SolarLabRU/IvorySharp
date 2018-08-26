using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Pipeline.StateMachine
{
    /// <summary>
    /// Базовая модель состояния выполнения пайплайна, характеризующаяся вызовом
    /// точки прикрепления аспектов типа <see cref="MethodBoundaryAspect"/>.
    /// </summary>
    /// <typeparam name="TPipeline">Тип пайплайна.</typeparam>
    internal abstract class MethodBoundaryState<TPipeline> : IInvocationState<TPipeline> 
        where TPipeline : InvocationPipelineBase
    {
        /// <summary>
        /// Перечень аспектов для применения.
        /// </summary>
        internal IEnumerable<MethodBoundaryAspect> BoundaryAspects { get; }
        
        /// <summary>
        /// Аспекты которые необходимо игнорировать.
        /// </summary>
        internal IReadOnlyCollection<MethodBoundaryAspect> IgnoredAspects { get; set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodBoundaryState{TPipeline}"/>.
        /// </summary>
        /// <param name="boundaryAspects">Перечень аспектов для применения.</param>
        protected MethodBoundaryState(IEnumerable<MethodBoundaryAspect> boundaryAspects)
        {
            BoundaryAspects = boundaryAspects;
            IgnoredAspects = Array.Empty<MethodBoundaryAspect>();
        }
        
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IInvocationState<TPipeline> MakeTransition(TPipeline pipeline)
        {
            foreach (var aspect in BoundaryAspects)
            {
                if (IgnoredAspects.Count > 0 && IgnoredAspects.ContainsReference(aspect))
                    continue;
                
                pipeline.ExecutionStateKey = aspect.InternalId;

                Apply(aspect, pipeline);

                if (ShouldBreak(pipeline, aspect, out var transition))
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
        /// <param name="aspect">Текущий аспект.</param>
        /// <param name="transition">Состояние, в которое необходимо выполнить переход.</param>
        /// <param name="pipeline">Пайплайн.</param>
        /// <returns>Признак того, что необходимо прервать выполнение текущего действия.</returns>
        protected abstract bool ShouldBreak(TPipeline pipeline, MethodBoundaryAspect aspect, out IInvocationState<TPipeline> transition);

        /// <summary>
        /// Создает переход к следующему состоянию (при успешном выполнении действия).
        /// </summary>
        /// <returns>Результирующий переход.</returns>
        protected abstract IInvocationState<TPipeline> CreateContinuation();    
    }
}