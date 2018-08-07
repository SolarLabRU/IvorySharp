using System;

namespace IvorySharp.Tests.Assets.Aspects
{
    public class ArgumentExceptionThrowAspect : ThrowAspect
    {
        public ArgumentExceptionThrowAspect(BoundaryType boundaryType) 
            : base(typeof(ArgumentException), boundaryType)
        {
        }
    }
}