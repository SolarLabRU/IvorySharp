using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ReturnValueAspect : ObservableAspect
    {
        private readonly BoundaryType _boundaryType;
        private readonly object _valueToReturn;

        public ReturnValueAspect(BoundaryType boundaryType, object valueToReturn)
        {
            _boundaryType = boundaryType;
            _valueToReturn = valueToReturn;
        }
            
        protected override void Entry(IInvocationPipeline pipeline)
        {
            base.Entry(pipeline);
            if (_boundaryType == BoundaryType.Entry)
            {
                pipeline.ReturnValue(_valueToReturn);
            }
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            base.OnSuccess(pipeline);
            if (_boundaryType == BoundaryType.Success)
            {
                pipeline.ReturnValue(_valueToReturn);
            }
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            base.Exception(pipeline);
            if (_boundaryType == BoundaryType.Exception)
            {
                pipeline.ReturnValue(_valueToReturn);
            }
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            base.Exit(pipeline);
            if (_boundaryType == BoundaryType.Exit)
            {
                pipeline.ReturnValue(_valueToReturn);
            }      
        }
    }
}