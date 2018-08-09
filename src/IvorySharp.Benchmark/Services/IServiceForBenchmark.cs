using IvorySharp.Benchmark.Aspects;

namespace IvorySharp.Benchmark.Services
{
    public interface IServiceForBenchmark
    {
        [BypassAspect]
        int Identity(int value);
    }
}