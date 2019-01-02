using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    
    internal class RethrowAspect : ObservableAspect
    {
        private readonly Type _exceptionType;

        public RethrowAspect(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            pipeline.ContinueFaulted(CreateException(_exceptionType, pipeline.CurrentException));
        }

        protected static Exception CreateException(Type exceptionType, Exception inner)
        {
            return (Exception) Activator.CreateInstance(exceptionType, String.Empty, inner);                       
        }
    }
}