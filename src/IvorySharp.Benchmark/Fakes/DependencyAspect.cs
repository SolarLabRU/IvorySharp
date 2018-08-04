using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Benchmark.Fakes
{
    public class DependencyAspect : MethodBoundaryAspect
    {
        [Dependency]
        public IDependencyService Dependency { get; set; }
    }
}