using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IMultipleBoundaryAspectsService
    {
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnExit), Order = 1)]
        [Pow2ValueAspect(Order = 0)]
        int Identity(int argument);
    }
}