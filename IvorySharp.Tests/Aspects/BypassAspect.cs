using System;

namespace IvorySharp.Tests.Aspects
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class BypassAspect : ObservableBoundaryAspect
    {      
    }
}