using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace Examples.ExceptionHandling
{
    /// <summary>
    /// Аспект выполняет логирования исключения и прерывает его выбрасывание наружу.
    /// </summary>
    public sealed class SwallowExceptionAspectAttribute : MethodBoundaryAspect
    {
        /// <inheritdoc />
        public override void OnException(IInvocationPipeline pipeline)
        {
            Console.WriteLine("SwallowExceptionAspect OnException called");
            
            Console.WriteLine($"Exception '{pipeline.CurrentException.GetType().Name}'; " +
                              $"Exception Message: '{pipeline.CurrentException.Message}'");

            var innerException = pipeline.CurrentException.InnerException;
            if (innerException != null)
            {
                Console.WriteLine($"InnerException '{innerException.GetType().Name}'; " +
                                  $"InnerException Message: '{innerException.Message}'");
            }
            
            pipeline.Continue();
        }
    }
}