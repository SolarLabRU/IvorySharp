﻿using System;
using System.Collections.Generic;
using IvorySharp.Aspects.Pipeline.Appliers;

namespace IvorySharp.Aspects.Pipeline
{
    /// <summary>
    /// Компонент, агрегирующий обработку коллекции аспектов.
    /// </summary>
    internal class MethodAspectReducer
    {
        private readonly InvocationPipelineBase _pipeline;

        /// <summary>
        /// Инициализирует экземпляр <see cref="MethodAspectReducer"/>.
        /// </summary>
        /// <param name="pipeline">Пайлпайн.</param>
        public MethodAspectReducer(InvocationPipelineBase pipeline)
        {
            _pipeline = pipeline;
        }
        
        /// <summary>
        /// Выполняет обработку аспектов <paramref name="aspects"/>
        /// с применением компонента <paramref name="applier"/>.
        /// </summary>
        /// <param name="aspects">Коллекция аспектов для применения.</param>
        /// <param name="applier">Компонент, выполняющий применение аспекта на пайплайн вызова.</param>
        /// <returns>Результат применения аспектов.</returns>
        public AspectApplyResult Reduce(IEnumerable<MethodBoundaryAspect> aspects, AspectApplier applier)
        {
            return ReduceBefore(aspects, applier, wall: null);
        }

        /// <summary>
        /// Выполняет обработку аспектов <paramref name="aspects"/>, расположенных
        /// в коллекции до аспекта <paramref name="wall"/>. То есть, у которых
        /// значение порядка выше (<see cref="OrderableMethodAspect.InternalOrder"/>) больше, чем
        /// у <paramref name="wall"/>. 
        /// с применением компонента <paramref name="applier"/>.
        /// </summary>
        /// <param name="aspects">Коллекция аспектов.</param>
        /// <param name="applier">Компонент, выполняющий применение аспекта на пайплайн вызова.</param>
        /// <param name="wall">Аспект - барьер.</param>
        /// <returns>Результат применения аспектов.</returns>
        public AspectApplyResult ReduceBefore(
            IEnumerable<MethodBoundaryAspect> aspects,
            AspectApplier applier, 
            MethodBoundaryAspect wall)
        {
            foreach (var aspect in aspects)
            {
                try
                {
                    _pipeline.CurrentExecutingAspect = aspect;

                    if (!applier.CanApply(_pipeline)) 
                        continue;
                    
                    if (wall != null)
                    {
                        if (aspect.InternalOrder >= wall.InternalOrder)
                            continue;  
                    }

                    var result = applier.Apply(aspect, _pipeline);
                    if (result.IsExecutionBreaked)
                        return result;
                }
                catch (Exception e)
                {
                    _pipeline.Fault(e);
                    return new AspectApplyResult(aspect);
                }
            }
            
            return new AspectApplyResult();
        }
    }
}