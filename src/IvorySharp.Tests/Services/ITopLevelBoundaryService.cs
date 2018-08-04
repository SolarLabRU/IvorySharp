using IvorySharp.Aspects;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    [IncrementValueAspect(nameof(MethodBoundaryAspect.OnExit), Order = 1)]
    public interface ITopLevelBoundaryService
    {
        [Pow2ValueAspect(Order = 2)]
        int Identity(int arg);
        
        [SuppressAspectsWeaving]
        int Identity2(int arg);
        
        [Pow2ValueAspect(Order = 0)]
        int Identity3(int arg);

        int Identity4(int arg);
    }

    public class TopLevelBoundaryService : ITopLevelBoundaryService
    {
        public int Identity(int arg)
        {
            return arg;
        }

        public int Identity2(int arg)
        {
            return arg;
        }

        public int Identity3(int arg)
        {
            return arg;
        }

        public int Identity4(int arg)
        {
            return arg;
        }
    }
}