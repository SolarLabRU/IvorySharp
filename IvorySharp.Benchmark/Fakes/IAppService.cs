using System.Runtime.CompilerServices;

namespace IvorySharp.Benchmark.Fakes
{
    public interface IAppService
    {
        [BypassAspect]
        int Identity(int argument);
    }

    public class AppService : IAppService
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public int Identity(int argument)
        {
            return argument;
        }
    }
}