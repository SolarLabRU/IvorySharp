using System;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Helpers;

namespace IvorySharp.Tests.Aspects
{
    public class RethrowExceptionAspect : ObservableBoundaryAspect
    {
        private readonly Type _exceptionType;
        private readonly BoundaryType _boundaryType;
        
        public RethrowExceptionAspect(Type exceptionType, BoundaryType boundaryType)
        {
            _exceptionType = exceptionType;
            _boundaryType = boundaryType;
        }
        
        protected override void Exception(IInvocationPipeline pipeline)
        {
            if (_boundaryType == BoundaryType.Exception)
            {
                var exception = (Exception) Activator.CreateInstance(_exceptionType);
                pipeline.RethrowException(exception);
            }
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            if (_boundaryType == BoundaryType.Entry)
            {
                var exception = (Exception) Activator.CreateInstance(_exceptionType);
                pipeline.RethrowException(exception);
            }
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            if (_boundaryType == BoundaryType.Exit)
            {
                var exception = (Exception) Activator.CreateInstance(_exceptionType);
                pipeline.RethrowException(exception);
            }
        }

        protected override void Success(IInvocationPipeline pipeline)
        {
            if (_boundaryType == BoundaryType.Success)
            {
                var exception = (Exception) Activator.CreateInstance(_exceptionType);
                pipeline.RethrowException(exception);
            }
        }
    }
}