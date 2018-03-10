using System.Runtime.CompilerServices;

namespace IvorySharp.Benchmark.Fakes
{

    public interface IDependencyAppService
    {       
        [DependencyAspect]
        int Identity(int argument);
    }
    
    public class DependencyAppService : IDependencyAppService
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public int Identity(int argument)
        {
            return argument;
        }
    }
}