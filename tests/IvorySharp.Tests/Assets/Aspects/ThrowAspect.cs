using System;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ThrowAspect : ObservableAspect
    {
        private readonly Type _exceptionType;
        private readonly BoundaryType _boundaryType;
        private readonly bool _throwAsUnhandled;

        public ThrowAspect(Type exceptionType, BoundaryType boundaryType, bool throwAsUnhandled)
        {
            _exceptionType = exceptionType;
            _boundaryType = boundaryType;
            _throwAsUnhandled = throwAsUnhandled;
        }
        
        public ThrowAspect(Type exceptionType, BoundaryType boundaryType)
            : this(exceptionType, boundaryType, throwAsUnhandled: false)
        {
        }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            base.Entry(pipeline);
            if (_boundaryType == BoundaryType.Entry)
            {
                ThrowException(CreateException(_exceptionType), pipeline);
            }
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            base.OnSuccess(pipeline);
            if (_boundaryType == BoundaryType.Success)
            {
                ThrowException(CreateException(_exceptionType), pipeline);
            }
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            base.Exception(pipeline);
            if (_boundaryType == BoundaryType.Exception)
            {
                ThrowException(CreateException(_exceptionType), pipeline);
            }
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            base.Exit(pipeline);
            if (_boundaryType == BoundaryType.Exit)
            {
                ThrowException(CreateException(_exceptionType), pipeline);
            }      
        }

        protected void ThrowException(Exception exception, IInvocationPipeline pipeline)
        {
            if (_throwAsUnhandled)
                throw exception;
            
            pipeline.Throw(exception);
        }

        protected static Exception CreateException(Type exceptionType)
        {
            return (System.Exception) Activator.CreateInstance(exceptionType);                       
        }
    }
}