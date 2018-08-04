using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components.Dependency;

namespace IvorySharp.Benchmark.Fakes
{
    public class DependencyAspect : MethodBoundaryAspect
    {
        [InjectDependency]
        public IDependencyService Dependency { get; set; }
    }
}