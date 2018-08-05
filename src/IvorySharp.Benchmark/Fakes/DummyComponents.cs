using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;

namespace IvorySharp.Benchmark.Fakes
{
    internal class DummyComponents : IComponentsStore
    {
        public IDependencyProvider DependencyProvider { get; set; }
        public IAspectSelector AspectSelector => new DefaultAspectSelector();
        public IAspectWeavePredicate AspectWeavePredicate => new CachedWeavePredicate(new DeclaringTypeWeavePredicate(AspectSelector));
        public IAspectDeclarationCollector AspectDeclarationCollector => new DeclaringTypeAspectDeclarationCollector(AspectSelector);
        public IPipelineExecutor AspectPipelineExecutor => AspectInvocationPipelineExecutor.Instance;
        public IAspectFactory AspectFactory => new AspectFactory(AspectDeclarationCollector, AspectDependencyInjector, AspectOrderStrategy);
        public IAspectDependencyInjector AspectDependencyInjector => new AspectDependencyInjector(DependencyProvider);
        public IAspectOrderStrategy AspectOrderStrategy => new DefaultAspectOrderStrategy();
    }
}