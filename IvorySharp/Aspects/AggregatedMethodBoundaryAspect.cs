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
        private IReadOnlyCollection<IMethodBoundaryAspect> _boundaryAspects;
        private IMethodBoundaryAspect _currentExecutingBoundary;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref=""/>.
        /// </summary>
        /// <param name="boundaryAspects"></param>
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
        /// <param name="boundary">Наименование обработчика.</param>
        /// <param name="consumer">Обработчик.</param>
        internal void IterateAspectsBeforeCurrent(
            IInvocationPipeline pipeline, string boundary, Action<IMethodBoundaryAspect, IInvocationPipeline> consumer)
        {
            if (_currentExecutingBoundary == null)
                return;

            var previousAspects = _boundaryAspects.TakeWhile(b => b != _currentExecutingBoundary);
            try
            {
                foreach (var aspect in previousAspects)
                {
                    if (InvocationPipelineFlow.CanContinueBoundary(pipeline, boundary))
                    {
                        consumer(aspect, pipeline);
                    }
                }
            }
            catch (Exception e)
            {
                pipeline.ThrowException(e);
            }
        }

        /// <summary>
        /// Выполняет обход всех аспектов.
        /// </summary>
        /// <param name="pipeline">Пайплайн.</param>
        /// <param name="consumer">Обработчик.</param>
        /// <param name="boundary">Наименование обработчика. Заполняется само, инициализировать не надо.</param>
        private void IterateAspects(
            IInvocationPipeline pipeline,
            Action<IMethodBoundaryAspect, IInvocationPipeline> consumer,
            [CallerMemberName] string boundary = "")
        {
            try
            {
                foreach (var aspect in _boundaryAspects)
                {
                    if (InvocationPipelineFlow.CanContinueBoundary(pipeline, boundary))
                    {
                        _currentExecutingBoundary = aspect;
                        consumer(aspect, pipeline);
                    }
                }
            }
            catch (Exception e)
            {
                pipeline.ThrowException(e);
            }
        }
    }
}