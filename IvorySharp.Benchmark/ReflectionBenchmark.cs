using System.Reflection;
using BenchmarkDotNet.Attributes;
using IvorySharp.Reflection;

namespace IvorySharp.Benchmark
{
    public class ReflectionBenchmark
    {
        /// <summary>
        /// Ссылка на метод получения типа.
        /// </summary>
        private static readonly MethodInfo GetTypeMethod 
            = typeof(object).GetMethod("GetType");

        private object _instance;

        [GlobalSetup]
        public void Setup()
        {
            _instance = new object();
        }
        
        [Benchmark]
        public void Dispatch_Using_MehodInvoke()
        {
            GetTypeMethod.Invoke(_instance, null);
        }

        [Benchmark]
        public void Dispatch_Using_Expressions()
        {
            var invoker = Expressions.CreateMethodInvoker(GetTypeMethod);
            invoker(_instance, null);
        }
    }
}