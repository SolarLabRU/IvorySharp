using System;
using IvorySharp.Tests.Aspects;
using IvorySharp.Tests.Helpers;

namespace IvorySharp.Tests.Services
{
    public interface IRethrowExceptionService
    {
        [RethrowExceptionAspect(typeof(ArgumentException), BoundaryType.Exception, Order = 0)]
        [SwallowExceptionIfTypeMatchAspect(typeof(ArgumentException), Order = 1)]
        void RethrowArgumentExceptionThenSwallow();

        [RethrowExceptionAspect(typeof(ArgumentException), BoundaryType.Exception)]
        void RethrowArgumentException();

        [RethrowExceptionAspect(typeof(ArgumentException), BoundaryType.Entry, Order = 0)]
        [SwallowExceptionAspect42Result]
        int RehrowOnEntryThenSwallowReturn42();
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

        public int RehrowOnEntryThenSwallowReturn42()
        {
            return 0;
        }
    }
}