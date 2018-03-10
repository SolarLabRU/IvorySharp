using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class PipelineThrowAspect : ObservableBoundaryAspect
    {
        protected override void Entry(IInvocationPipeline pipeline)
        {
            pipeline.ThrowException(new Exception());
        }
    }
}