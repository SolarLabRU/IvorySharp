using IvorySharp.Aspects;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Tests.Aspects;
using IvorySharp.Tests.Helpers;

namespace IvorySharp.Tests.Services
{
    [EnableWeaving]
    public interface IMarkedExplicitBoundaryService
    {
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnExit))]
        int Identity(int arg);

        [SuppressAspectsWeaving]
        int Identity2(int arg);
    }

    public interface INotMarkedExplicitBoundaryService
    {
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnExit))]
        int Identity(int arg);

        [SuppressAspectsWeaving]
        int Identity2(int arg);
    }

    public class ExplicitBoundaryService : IMarkedExplicitBoundaryService, INotMarkedExplicitBoundaryService
    {
        int IMarkedExplicitBoundaryService.Identity(int arg)
        {
            return arg;
        }

        int INotMarkedExplicitBoundaryService.Identity2(int arg)
        {
            return arg;
        }

        int IMarkedExplicitBoundaryService.Identity2(int arg)
        {
            return arg;
        }

        int INotMarkedExplicitBoundaryService.Identity(int arg)
        {
            return arg;
        }
    }
}