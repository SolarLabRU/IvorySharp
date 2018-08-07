using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ThrowAspect : ObservableAspect
    {
        private readonly Type _exceptionType;
        private readonly BoundaryType _boundaryType;

        public ThrowAspect(Type exceptionType, BoundaryType boundaryType)
        {
            _exceptionType = exceptionType;
            _boundaryType = boundaryType;
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            base.Entry(pipeline);
            if (_boundaryType == BoundaryType.Entry)
            {
                throw CreateException(_exceptionType);
            }
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            base.OnSuccess(pipeline);
            if (_boundaryType == BoundaryType.Success)
            {
                throw CreateException(_exceptionType);
            }
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            base.Exception(pipeline);
            if (_boundaryType == BoundaryType.Exception)
            {
                throw CreateException(_exceptionType);
            }
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            base.Exit(pipeline);
            if (_boundaryType == BoundaryType.Exit)
            {
                throw CreateException(_exceptionType);
            }      
        }

        protected static Exception CreateException(Type exceptionType)
        {
            return (System.Exception) Activator.CreateInstance(exceptionType);                       
        }
    }
}