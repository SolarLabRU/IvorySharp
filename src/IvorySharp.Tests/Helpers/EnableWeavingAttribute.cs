using System;

namespace IvorySharp.Tests.Helpers
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class EnableWeavingAttribute : Attribute { }
}