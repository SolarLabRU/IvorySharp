using System;
using System.Collections.Generic;

namespace IvorySharp.Aspects.Pipeline.Visitors
{
    internal class InvocationPipelineVisitorAcceptor
    {
        private readonly InvocationPipeline _pipeline;

        public InvocationPipelineVisitorAcceptor(InvocationPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public VisitResult Accept(IEnumerable<MethodBoundaryAspect> aspects, InvocationPipelineVisitor boundaryVisitor)
        {
            return AcceptBefore(aspects, boundaryVisitor, wall: null, inclusive: true);
        }

        public VisitResult AcceptBefore(
            IEnumerable<MethodBoundaryAspect> aspects,
            InvocationPipelineVisitor boundaryVisitor, 
            MethodBoundaryAspect wall,
            bool inclusive = false)
        {
            foreach (var aspect in aspects)
            {
                try
                {
                    _pipeline.CurrentExecutingAspect = aspect;

                    if (!boundaryVisitor.CanVisit(_pipeline)) 
                        continue;
                    
                    if (wall != null)
                    {
                        var shouldSkip = inclusive
                            ? aspect.InternalOrder > wall.InternalOrder
                            : aspect.InternalOrder >= wall.InternalOrder;
                            
                        if (shouldSkip)
                            continue;  
                    }

                    var result = boundaryVisitor.Visit(aspect, _pipeline);
                    if (result.IsExecutionBreaked)
                        return result;
                }
                catch (Exception e)
                {
                    _pipeline.ThrowException(e);
                    return new VisitResult(aspect);
                }
            }
            
            return new VisitResult();
        }
    }
}