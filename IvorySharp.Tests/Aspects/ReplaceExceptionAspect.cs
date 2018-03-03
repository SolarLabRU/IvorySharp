using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class ReplaceExceptionAspect : ObservableBoundaryAspect
    {
        protected override void Exception(IInvocationPipeline pipeline)
        {
            pipeline.ThrowException(new ArgumentException());
        }
    }
}