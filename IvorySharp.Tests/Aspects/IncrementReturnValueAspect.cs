using System.Linq;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class IncrementReturnValueAspect : ObservableBoundaryAspect
    {
        private static string[] _appliedBoundaries = new string[0];

        public static void SetAppliedBoundaries(params string[] boundaries)
        {
            _appliedBoundaries = boundaries;
        }

        public static void ResetAppliedBoundaries()
        {
            _appliedBoundaries = new string[0];
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