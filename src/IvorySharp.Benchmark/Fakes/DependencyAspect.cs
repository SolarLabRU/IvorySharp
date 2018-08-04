using IvorySharp.Aspects;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Benchmark.Fakes
{
    public class DependencyAspect : MethodBoundaryAspect
    {
        [InjectDependency]
        public IDependencyService Dependency { get; set; }
    }
}