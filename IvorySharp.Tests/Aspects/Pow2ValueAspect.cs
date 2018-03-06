using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class Pow2ValueAspect : ObservableBoundaryAspect
    {
        protected override void Exit(IInvocationPipeline pipeline)
        {
            var retVal = (int) pipeline.Context.ReturnValue;
            pipeline.Context.ReturnValue = retVal * retVal;
        }
    }
}