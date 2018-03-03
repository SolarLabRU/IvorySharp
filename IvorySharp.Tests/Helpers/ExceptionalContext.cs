using System;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;

namespace IvorySharp.Tests.Helpers
{
    public class ExceptionalContext
    {
        public InvocationContext Source { get; set; }
        public Exception Exception { get; set; }
            
        public static ExceptionalContext FromPipeline(IInvocationPipeline pipeline)
        {
            return new ExceptionalContext
            {
                Source = pipeline.Context,
                Exception = pipeline.CurrentException
            };
        }
    }
}