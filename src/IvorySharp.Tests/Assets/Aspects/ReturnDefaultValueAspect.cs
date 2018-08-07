using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ReturnDefaultValueAspect : ObservableAspect
    {
        private readonly BoundaryType _boundaryType;

        public ReturnDefaultValueAspect(BoundaryType boundaryType)
        {
            _boundaryType = boundaryType;
        }
            
        protected override void Entry(IInvocationPipeline pipeline)
        {
            base.Entry(pipeline);
            if (_boundaryType == BoundaryType.Entry)
            {
                pipeline.Return();
            }
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            base.OnSuccess(pipeline);
            if (_boundaryType == BoundaryType.Success)
            {
                pipeline.Return();
            }
        }

        protected override void Exception(IInvocationPipeline pipeline)
        {
            base.Exception(pipeline);
            if (_boundaryType == BoundaryType.Exception)
            {
                pipeline.Return();
            }
        }

        protected override void Exit(IInvocationPipeline pipeline)
        {
            base.Exit(pipeline);
            if (_boundaryType == BoundaryType.Exit)
            {
                pipeline.Return();
            }      
        }
    }
}