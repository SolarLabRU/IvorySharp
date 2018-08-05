﻿using IvorySharp.Aspects;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Benchmark.Fakes
{
    public class DependencyAspect : MethodBoundaryAspect
    {
        [Dependency]
        public IDependencyService Dependency { get; set; }
    }
}