using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class Return42Aspect : ObservableBoundaryAspect
    {
        private readonly List<string> _appliedBoundaries;

        public Return42Aspect(params string[] appliedBoundaries)
        {
            _appliedBoundaries = appliedBoundaries.ToList();
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnEntry)))
                return;
            
            pipeline.ReturnValue(42);
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnException)))
            {
                pipeline.Return();
            }

            pipeline.ReturnValue(42);
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnExit)))
                return;
            
            pipeline.ReturnValue(42);
        }

        protected override void Success(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnSuccess)))
                return;
            
            pipeline.ReturnValue(42);
        }
    }
}