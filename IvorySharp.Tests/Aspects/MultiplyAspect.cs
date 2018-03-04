using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Services;

namespace IvorySharp.Tests.Aspects
{
    public class MultiplyAspect : ObservableBoundaryAspect
    {
        public override void OnExit(IInvocationPipeline pipeline)
        {
            var multiplyService = pipeline.ServiceProvider.GetService<IMultiplyService>();
            var retVal = (int) pipeline.Context.ReturnValue;

            pipeline.Context.ReturnValue = multiplyService.Multiply(retVal, 2);
        }
    }
}