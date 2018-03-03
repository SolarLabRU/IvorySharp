using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IBoundaryAspectsService
    {
        [BypassAspect]
        void BypassEmptyMethod();

        [IncrementReturnValueAspect]
        int Identity(int argument);

        [IncrementReturnValueAspect]
        int Identity2(int argument);

        [SwallowExceptionAspectDefaultReturn]
        void ExceptionalEmptyMethod();

        [BypassAspect]
        void ExceptionalEmptyMethod2();

        [ReplaceExceptionAspect]
        void ExceptionalEmptyMethod3();
        
        [SwallowExceptionAspectDefaultReturn]
        int ExceptionalIdentity(int argumetn);

        [SwallowExceptionAspect42Result]
        int ExceptionalIdentity2(int argument);

        [SwallowExceptionAspectDefaultReturn]
        object ExceptionalRef();

        [SwallowExceptionAspectNewObjectResult]
        object ExceptionalRef2();
    }
}