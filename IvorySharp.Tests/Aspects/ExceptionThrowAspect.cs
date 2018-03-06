using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class ExceptionThrowAspect : ObservableBoundaryAspect
    {
        private readonly List<string> _appliedBoundaries;

        public ExceptionThrowAspect(params string[] appliedBoundaries)
        {
            _appliedBoundaries = appliedBoundaries.ToList();
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnEntry)))
                return;
            
            throw new Exception();
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnException)))
            {
                pipeline.Return();
            }

            throw new Exception();
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnExit)))
                return;
            
            throw new Exception();
        }

        protected override void Success(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnSuccess)))
                return;
            
            throw new Exception();
        }
    }
}