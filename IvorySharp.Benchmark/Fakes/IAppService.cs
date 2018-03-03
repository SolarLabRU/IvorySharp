using System.Runtime.CompilerServices;

namespace IvorySharp.Benchmark.Fakes
{
    public interface IAppService
    {
        [BypassAspect]
        void VoidMethod();
    }

    public class AppService : IAppService
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public void VoidMethod()
        {
            string nothing = null;
            return;
        }
    }
}