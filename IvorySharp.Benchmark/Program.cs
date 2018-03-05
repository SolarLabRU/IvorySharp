using BenchmarkDotNet.Running;

namespace IvorySharp.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(BenchmarkProxyCreation).Assembly).Run();
        }
    }
}