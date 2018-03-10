using System;
using System.Linq;
using System.Text;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using Newtonsoft.Json;

namespace IvorySharp.Examples.Aspects
{
    /// <summary>
    /// Печатает список входных параметров. 
    /// </summary>
    public class TraceArgumentsAspect : MethodBoundaryAspect
    {
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            var sb = new StringBuilder();
            var @params = pipeline.Context.Method.GetParameters();

            sb.AppendLine($"Input args of '{pipeline.Context.Method.Name}' are [");
            
            for (var i = 0; i < pipeline.Context.Arguments.Count; i++)
            {
                var arg = pipeline.Context.Arguments.ElementAt(i);
                var param = @params[i];

                sb.AppendLine($"[{param.ParameterType.Name}]{param.Name}: {JsonConvert.SerializeObject(arg, Formatting.Indented)}");
            }

            sb.AppendLine("]");
            
            Console.Write(sb.ToString());
        }
    }
}