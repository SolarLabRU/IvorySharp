using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class SwallowExceptionAspectDefaultReturn : ObservableBoundaryAspect
    {
        protected override void Exception(IInvocationPipeline pipeline)
        {
            pipeline.ReturnDefault();
        }
    }
}