using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IMultipleBoundaryAspectsService
    {
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnExit), Order = 1)]
        [Pow2ValueAspect(Order = 0)]
        int Identity(int argument);

        [ExceptionThrowAspect(nameof(IMethodBoundaryAspect.OnEntry), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnEntry(int argument);
        
        [ExceptionThrowAspect(nameof(IMethodBoundaryAspect.OnSuccess), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnSuccess(int argument);
        
        [ExceptionThrowAspect(nameof(IMethodBoundaryAspect.OnExit), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnExit(int argument);

        [SwallowExceptionAspectDefaultReturn(Order = 0)]
        [BypassAspect(Order = 1)]
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnSuccess), Order = 2)]
        int IdentityInnerSuccess(int argument);
        
        [IncrementValueAspect(nameof(IMethodBoundaryAspect.OnSuccess), Order = 0)] // Этот выполнится
        [Return42Aspect(nameof(IMethodBoundaryAspect.OnEntry), Order = 1)] // Этот остановит пайплайн
        [BypassAspect(Order = 2)] // Этот не выполнится
        int IdentityReturnOnEntry(int argument);
    }
}