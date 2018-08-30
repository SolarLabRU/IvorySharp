using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace Examples.ExceptionHandling
{
    /// <summary>
    /// Аспект заменяет тип исключения на другой.
    /// </summary>
    public sealed class ReplaceExceptionAspectAttribute : MethodBoundaryAspect
    {
        /// <inheritdoc />
        public override void OnException(IInvocationPipeline pipeline)
        {
            Console.WriteLine("ReplaceExceptionAspect OnException called");
            
            pipeline.Continue(
                new ApplicationException("Additional exception info", pipeline.CurrentException));
        }
    }
}