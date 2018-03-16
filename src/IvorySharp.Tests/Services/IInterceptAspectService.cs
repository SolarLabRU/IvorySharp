using System;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IInterceptAspectService
    {
        [SwallowExceptionInterceptAspect]
        object ExceptionSwalloved();
    }

    public class InterceptAspectService : IInterceptAspectService
    {
        public object ExceptionSwalloved()
        {
            throw new Exception();
        }
    }
}