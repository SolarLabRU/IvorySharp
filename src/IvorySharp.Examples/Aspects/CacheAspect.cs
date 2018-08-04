using System;
using System.Linq;
using System.Text;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using Microsoft.Extensions.Caching.Memory;

namespace IvorySharp.Examples.Aspects
{
    /// <summary>
    /// Аспект кеширования.
    /// При вызове метода смотрит есть ли значение в кеше, и если есть - возвращает его вместо обращения к методу.
    /// </summary>
    public class CacheAspect : MethodBoundaryAspect
    {
        public static readonly MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
        
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            var cacheKey = GetCacheKey(pipeline.Context);
            if (MemoryCache.TryGetValue(cacheKey, out var cached))
            {
                pipeline.ReturnValue(cached);
                Console.WriteLine($"Return '{pipeline.Context.Method.ReturnType.Name}' with key '{cacheKey}' from cache");
            }
            else
            {
                pipeline.AspectExecutionState = cacheKey;
            }
        }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            var cacheKey = (string)pipeline.AspectExecutionState;
            MemoryCache.Set(cacheKey, pipeline.Context.ReturnValue);
            Console.WriteLine($"Set '{pipeline.Context.Method.ReturnType.Name}' to cache with key '{cacheKey}'");
        }

        private string GetCacheKey(InvocationContext context)
        {
            var sb = new StringBuilder();

            sb.Append(context.DeclaringType.FullName);
            sb.Append(".");
            sb.Append(context.Method.Name);
            sb.Append("(");
            for (var i = 0; i < context.Arguments.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.Append(context.Arguments.ElementAt(i));
            }
            sb.Append(')');

            return sb.ToString();
        }
    }
}