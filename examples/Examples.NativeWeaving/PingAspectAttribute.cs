using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace Examples.NativeWeaving
{
    public class PingAspectAttribute : MethodBoundaryAspect
    {
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            base.OnEntry(pipeline);
            Console.WriteLine("Aspect: OnEntry");
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            base.OnSuccess(pipeline);
            Console.WriteLine("Aspect: OnSuccess");
        }

        public override void OnExit(IInvocationPipeline pipeline)
        {
            base.OnExit(pipeline);
            Console.WriteLine("Aspect: OnExit");
        }
    }
}