using System.Threading.Tasks;
using IvorySharp.Benchmark.Aspects;

namespace IvorySharp.Benchmark.Services
{
    public interface IServiceForBenchmark
    {
        [BypassBoundaryAspect]
        int Identity(int value);

        [BypassBoundaryAspect]
        Task<int> IdentityAsync(int value);

        [BypassInterceptionAspect]
        int InterceptedIdentity(int value);
    }
}