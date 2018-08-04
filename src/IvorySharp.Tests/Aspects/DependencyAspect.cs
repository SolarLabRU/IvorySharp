using System;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Services;

namespace IvorySharp.Tests.Aspects
{
    public class DependencyAspect : ObservableBoundaryAspect
    {
        [InjectDependency]
        public IDependencyService Dependency { get; set; }

        protected override void Entry(IInvocationPipeline pipeline)
        {
            if (Dependency == null)
                throw new Exception();
            
            base.Entry(pipeline);
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            if (Dependency == null)
                throw new Exception();
            
            base.OnSuccess(pipeline);
        }

        public override void OnExit(IInvocationPipeline pipeline)
        {
            if (Dependency == null)
                throw new Exception();
            
            base.OnExit(pipeline);
        }
    }
}