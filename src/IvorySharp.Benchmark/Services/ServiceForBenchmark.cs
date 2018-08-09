namespace IvorySharp.Benchmark.Services
{
    public class ServiceForBenchmark : IServiceForBenchmark
    {
        public int Identity(int value)
        {
            return value;
        }
    }
}