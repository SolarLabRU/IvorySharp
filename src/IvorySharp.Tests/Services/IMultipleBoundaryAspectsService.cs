using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IMultipleBoundaryAspectsService
    {
        [IncrementValueAspect(nameof(MethodBoundaryAspect.OnExit), Order = 1)]
        [Pow2ValueAspect(Order = 0)]
        int Identity(int argument);

        [ExceptionThrowAspect(nameof(MethodBoundaryAspect.OnEntry), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnEntry(int argument);
        
        [ExceptionThrowAspect(nameof(MethodBoundaryAspect.OnSuccess), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnSuccess(int argument);
        
        [ExceptionThrowAspect(nameof(MethodBoundaryAspect.OnExit), Order = 2)]
        [SwallowExceptionAspect42Result(Order = 1)]
        int IdentityThrowOnExit(int argument);

        [SwallowExceptionAspectDefaultReturn(Order = 0)]
        [BypassAspect(Order = 1)]
        [IncrementValueAspect(nameof(MethodBoundaryAspect.OnSuccess), Order = 2)]
        int IdentityInnerSuccess(int argument);
        
        [IncrementValueAspect(nameof(MethodBoundaryAspect.OnSuccess), Order = 0)] // Этот выполнится
        [Return42Aspect(nameof(MethodBoundaryAspect.OnEntry), Order = 1)] // Этот остановит пайплайн
        [BypassAspect(Order = 2)] // Этот не выполнится
        int IdentityReturnOnEntry(int argument);

        [BypassAspect(Order = 0)] // Не вызовется от Success, но вызовется Exit
        [PipelineThrowAspect(Order = 1)] // Этот выбросит исключение и остановит пайплайн
        [Return42Aspect(nameof(MethodBoundaryAspect.OnExit), nameof(MethodBoundaryAspect.OnSuccess), Order = 2)] // Не выполнится
        int IdentityThrowPipelineOnEntry(int argument);
    }
}