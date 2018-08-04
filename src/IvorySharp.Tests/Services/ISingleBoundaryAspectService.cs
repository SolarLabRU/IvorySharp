using System;
using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface ISingleBoundaryAspectService
    {
        [BypassAspect]
        void BypassEmptyMethod();

        [IncrementValueAspect(
            nameof(MethodBoundaryAspect.OnExit), 
            nameof(MethodBoundaryAspect.OnSuccess))]
        int Identity(int argument);

        [IncrementValueAspect(nameof(MethodBoundaryAspect.OnEntry))]
        int Identity2(int argument);

        [MultiplyAspect]
        int Identity3(int argument);

        [SwallowExceptionAspectDefaultReturn]
        void ExceptionalEmptyMethod();

        [BypassAspect]
        void ExceptionalEmptyMethod2();

        [ThrowExceptionAspect(typeof(ArgumentException))]
        void ExceptionalEmptyMethod3();
        
        [SwallowExceptionAspectDefaultReturn]
        int ExceptionalIdentity(int argumetn);

        [SwallowExceptionAspect42Result]
        int ExceptionalIdentity2(int argument);

        [SwallowExceptionAspectDefaultReturn]
        object ExceptionalRef();

        [SwallowExceptionAspectNewObjectResult]
        object ExceptionalRef2();

        [DependencyAspect]
        int DependencyIdentity(int argument);
    }
}