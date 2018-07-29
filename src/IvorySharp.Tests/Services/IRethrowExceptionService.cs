using System;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IRethrowExceptionService
    {
        [RethrowExceptionAspect(typeof(ArgumentException), Order = 0)]
        [SwallowExceptionIfTypeMatchAspect(typeof(ArgumentException), Order = 1)]
        void RethrowArgumentExceptionThenSwallow();

        [RethrowExceptionAspect(typeof(ArgumentException))]
        void RethrowArgumentException();
    }

    public class RethrowExceptionService : IRethrowExceptionService
    {
        public void RethrowArgumentExceptionThenSwallow()
        {
            throw new ApplicationException();
        }

        public void RethrowArgumentException()
        {
            throw new ApplicationException();
        }
    }
}