using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class SwallowExceptionAspectNewObjectResult : ObservableBoundaryAspect
    {
        /// <inheritdoc />
        protected override void Exception(IInvocationPipeline pipeline)
        {
            pipeline.ReturnValue(new object());
        }
    }
}