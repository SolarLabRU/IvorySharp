using System;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using Newtonsoft.Json;

namespace Examples.Logging
{
    /// <summary>
    /// Выполняет логирование аргрументов аспекта.
    /// </summary>
    public sealed class LogMethodArgumentsAspectAttribute : MethodBoundaryAspect
    {
        /// <inheritdoc />
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            var str = JsonConvert.SerializeObject(pipeline.Context.Arguments.ElementAt(0), Formatting.Indented);
            Console.WriteLine($"LogMethodAspect: Method called with first arg : {str}");
        }
    }
}