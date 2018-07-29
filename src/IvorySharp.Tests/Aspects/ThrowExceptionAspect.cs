using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class ThrowExceptionAspect : ObservableBoundaryAspect
    {
        private readonly Type _exceptionType;

        public ThrowExceptionAspect(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            var exc = (Exception)Activator.CreateInstance(_exceptionType);
            pipeline.ThrowException(exc);
        }
    }
}