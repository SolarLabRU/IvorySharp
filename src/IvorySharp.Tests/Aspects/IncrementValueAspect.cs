using System;
using System.Linq;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public class IncrementValueAspect : ObservableBoundaryAspect
    {
        private string[] _appliedBoundaries;

        public IncrementValueAspect(params string[] appliedBoundaries)
        {
            _appliedBoundaries = appliedBoundaries;
        }

        protected override void Success(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnSuccess)))
                return;
            
            var retVal = (int) pipeline.Context.ReturnValue;
            pipeline.Context.ReturnValue = ++retVal;
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnExit)))
                return;
            
            var retVal = (int) pipeline.Context.ReturnValue;
            pipeline.Context.ReturnValue = ++retVal;
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            if (!_appliedBoundaries.Contains(nameof(OnEntry)))
                return;
            
            var retVal = (int?) pipeline.Context.ReturnValue;
            pipeline.Context.ReturnValue = ++retVal;
        }
    }
}