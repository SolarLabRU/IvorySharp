using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components.Dependency;
using IvorySharp.Tests.Services;

namespace IvorySharp.Tests.Aspects
{
    public class MultiplyAspect : ObservableBoundaryAspect
    {
        [InjectDependency]
        public IMultiplyService  MultiplyService { get; set; }
        
        public override void OnExit(IInvocationPipeline pipeline)
        {
            var retVal = (int) pipeline.Context.ReturnValue;

            pipeline.Context.ReturnValue = MultiplyService.Multiply(retVal, 2);
        }
    }
}