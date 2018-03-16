using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Агрегированный аспект для множества аспектов <see cref="IMethodBoundaryAspect"/>.
    /// </summary>
    internal class AggregatedMethodBoundaryAspect : MethodBoundaryAspect
    {
        private readonly IReadOnlyCollection<IMethodBoundaryAspect> _boundaryAspects;
        private IMethodBoundaryAspect _currentExecutingBoundary;
        private int? _triggeredStopPipelineAspectOrder;
        
        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AggregatedMethodBoundaryAspect"/>.
        /// </summary>
        /// <param name="boundaryAspects">Коллекция аспектов.</param>
        internal AggregatedMethodBoundaryAspect(IReadOnlyCollection<IMethodBoundaryAspect> boundaryAspects)
        {
            _boundaryAspects = boundaryAspects.IsNotEmpty() ? boundaryAspects : Array.Empty<IMethodBoundaryAspect>();
        }

        /// <inheritdoc />
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            IterateAspects(pipeline, (a, p) => a.OnEntry(p));
        }

        /// <inheritdoc />
        public override void OnException(IInvocationPipeline pipeline)
        {
            IterateAspects(pipeline, (a, p) => a.OnException(p));
        }

        /// <inheritdoc />
        public override void OnExit(IInvocationPipeline pipeline)
        {
            IterateAspects(pipeline, (a, p) => a.OnExit(p));
        }

        /// <inheritdoc />
        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            IterateAspects(pipeline, (a, p) => a.OnSuccess(p));
        }

        /// <summary>
        /// Выполняет обход всех аспектов, до последнего выполненного
        /// </summary>
        /// <param name="pipeline">Пайплайн.</param>
        /// <param name="boundaryName">Наименование обработчика.</param>
        /// <param name="boundary">Обработчик.</param>
        internal void IterateAspectsBeforeLastApplied(
            IInvocationPipeline pipeline, string boundaryName, Action<IMethodBoundaryAspect, IInvocationPipeline> boundary)
        {
            // Если ничего не выполнилось, то и обходить нечего.
            if (_currentExecutingBoundary == null)
                return;

            var previousAspects = _boundaryAspects.Reverse().TakeWhile(b => b != _currentExecutingBoundary);
            var upcastedPipeline = (InvocationPipeline) pipeline;
            
            try
            {
                foreach (var aspect in previousAspects)
                {
                    upcastedPipeline.CurrentExecutingAspect = aspect;
                    
                    if (InvocationPipelineFlow.CanContinueBoundary(upcastedPipeline, boundaryName))
                    {
                        boundary(aspect, upcastedPipeline);
                    }
                }
            }
            catch (Exception e)
            {
                upcastedPipeline.ThrowException(e);
            }
        }

        /// <summary>
        /// Выполняет обход всех аспектов.
        /// </summary>
        /// <param name="pipeline">Пайплайн.</param>
        /// <param name="boundary">Обработчик.</param>
        /// <param name="boundaryName">Наименование обработчика. Заполняется само, инициализировать не надо.</param>
        private void IterateAspects(
            IInvocationPipeline pipeline,
            Action<IMethodBoundaryAspect, IInvocationPipeline> boundary,
            [CallerMemberName] string boundaryName = "")
        {
            var upcastedPipeline = (InvocationPipeline) pipeline;
         
            try
            {
                foreach (var aspect in _boundaryAspects)
                {
                    upcastedPipeline.CurrentExecutingAspect = aspect;
                    
                    if (InvocationPipelineFlow.CanContinueBoundary(upcastedPipeline, boundaryName))
                    {
                        // Если пайплайн был прерван, то последующие обработчики выполняться не должны
                        if (_triggeredStopPipelineAspectOrder.HasValue &&
                            aspect.Order >= _triggeredStopPipelineAspectOrder.Value)
                        {
                            continue;
                        }
                             
                        _currentExecutingBoundary = aspect;
                        
                        boundary(aspect, upcastedPipeline);

                        if (!_triggeredStopPipelineAspectOrder.HasValue)
                        {
                            if (pipeline.FlowBehaviour == FlowBehaviour.ThrowException ||
                                pipeline.FlowBehaviour == FlowBehaviour.Return)
                            {
                                _triggeredStopPipelineAspectOrder = aspect.Order;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                upcastedPipeline.ThrowException(e);
            }
        }
    }
}