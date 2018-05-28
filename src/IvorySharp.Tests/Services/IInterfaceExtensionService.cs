using System;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    [SwallowExceptionAspectDefaultReturn]
    public interface IInterfaceExtensionServiceParent
    {
        void DoSomethingBase();
    }

    public interface IInterfaceExtensionService : IInterfaceExtensionServiceParent
    {
        [SwallowExceptionAspectDefaultReturn]
        void DoSomething();
    }

    public class InterfaceExtensionService : IInterfaceExtensionService
    {
        public void DoSomethingBase()
        {
            throw new Exception();
        }

        public void DoSomething()
        {
            throw new Exception();
        }
    }
}