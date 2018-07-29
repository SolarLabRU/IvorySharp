using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Aspects
{
    public class RethrowExceptionAspect : ObservableBoundaryAspect
    {
        private readonly Type _exceptionType;
        
        public RethrowExceptionAspect(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            var exception = (Exception)Activator.CreateInstance(_exceptionType);
            pipeline.RethrowException(exception);
        }
    }
}