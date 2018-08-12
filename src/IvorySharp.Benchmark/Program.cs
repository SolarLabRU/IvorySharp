using BenchmarkDotNet.Running;

namespace IvorySharp.Benchmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(BenchmarkDispatch).Assembly)
                .Run(args);
        }
    }
}