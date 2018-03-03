using System;

namespace IvorySharp.Aspects
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SupressAspectAttribute : AspectAttribute
    { 
    }
}