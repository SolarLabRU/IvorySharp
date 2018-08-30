using System;
using System.Linq;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using Microsoft.Extensions.Caching.Memory;

namespace Examples.Caching
{
    /// <summary>
    /// Аспект выполняет кеширование возрващаемого методом значения по ключу.
    /// Если значения в кеше нет - метод выполняется и значение сохраняется в кеш,
    /// иначе - клиенту возвращается значение из кеша, а выполнение метода не происходит.
    /// </summary>
    public sealed class CacheAspectAttribute : MethodBoundaryAspect
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        
        /// <inheritdoc />
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            var cacheKey = CreateCacheKey(pipeline);

            // Если смогли достать значение - прерываем пайплайн вызовом ReturnValue
            if (Cache.TryGetValue(cacheKey, out var cached))
            {
                Console.WriteLine($"Aspect OnEntry: Return '{pipeline.Context.Method.ReturnType.Name}' with key '{cacheKey}' from cache");
                pipeline.Return(cached);
            }
            // Значение нет в кеше
            else
            {
                Console.WriteLine($"Aspect OnEntry: Key '{cacheKey}' not found in cache. Calling method...");
                // Записываем в состояние ключ кеша для сохранения значения
                pipeline.ExecutionState = cacheKey;
            }
        }

        /// <inheritdoc />
        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            // Если по Id-нику смогли что-то найти
            if (pipeline.CurrentReturnValue != null)
            {
                var cacheKey = (string) pipeline.ExecutionState;
                Cache.Set(cacheKey, pipeline.CurrentReturnValue);
                Console.WriteLine($"Aspect OnExit: Set '{pipeline.Context.Method.ReturnType.Name}' to cache with key '{cacheKey}'");
            }
            else
            {
                Console.WriteLine("Aspect OnExit: SUnable to save value to cache"); 
            }
        }

        private string CreateCacheKey(IInvocationPipeline pipeline)
        {
            var arg = (int) pipeline.Context.Arguments.ElementAt(0);
            return arg.ToString();
        }
    }
}