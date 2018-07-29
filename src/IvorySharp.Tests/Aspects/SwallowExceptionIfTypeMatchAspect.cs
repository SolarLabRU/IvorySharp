using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class SwallowExceptionIfTypeMatchAspect : ObservableBoundaryAspect
    {
        private readonly Type _expectedExceptionType;

        public SwallowExceptionIfTypeMatchAspect(Type expectedExceptionType)
        {
            _expectedExceptionType = expectedExceptionType;
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            if (pipeline.CurrentException.GetType() == _expectedExceptionType)
                pipeline.Return();
            
        }
    }
}