using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class SwallowExceptionAspect42Result : ObservableBoundaryAspect
    {
        /// <inheritdoc />
        protected override void Exception(IInvocationPipeline pipeline)
        {
            pipeline.ReturnValue(42);
        }
    }
}