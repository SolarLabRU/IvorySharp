using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Assets;

namespace IvorySharp.Tests.UnitTests
{
    public partial class AspectInvocationPipelineExecutorTests
    {
        private AspectInvocationPipeline CreatePipeline<TService>(
            TService instace, 
            string methodName,  
            MethodBoundaryAspect[] boundaryAspects,
            params object[] arguments)
        {
            return new AspectInvocationPipeline(
                new ObservableInvocation(typeof(TService), instace, methodName, arguments), 
                boundaryAspects, BypassMethodAspect.Instance);
        }
    }
}