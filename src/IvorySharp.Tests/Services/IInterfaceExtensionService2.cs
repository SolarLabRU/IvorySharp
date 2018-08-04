using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IInterfaceExtensionService2 
        : IInterfaceTypeExtensionServiceBase2, IInterfaceMethodExtensionServiceBase2
    {
    }

    public interface IInterfaceTypeExtensionServiceBase2
    {
        [SwallowExceptionAspect42Result]
        int DoSomethingChild();
    }

    public interface IInterfaceMethodExtensionServiceBase2
    {
        [SwallowExceptionAspectDefaultReturn]
        void DoSomethingBase();

        [SuppressAspectsWeaving]
        void SuppressedMethod();
    }

    public class InterfaceExtensionService2 : IInterfaceExtensionService2
    {
        public int DoSomethingChild()
        {
            throw new Exception();
        }

        public void DoSomethingBase()
        {
            throw new Exception();
        }

        public void SuppressedMethod()
        {
            throw new Exception();
        }
    }
}
