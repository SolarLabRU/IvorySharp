using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Benchmark.Fakes
{
    internal class DummyComponents : IComponentsStore
    {
        public IDependencyProvider DependencyProvider { get; set; }
        public IAspectSelector AspectSelector => new DefaultAspectSelector();
        public IAspectWeavePredicate AspectWeavePredicate => new DeclaringTypeWeavePredicate(AspectSelector);
        public IAspectDeclarationCollector AspectDeclarationCollector => new DeclaringTypeAspectDeclarationCollector(AspectSelector);
        public IPipelineExecutor AspectPipelineExecutor => AspectInvocationPipelineExecutor.Instance;
        public IAspectFactory AspectFactory => new AspectFactory(AspectDeclarationCollector, AspectDependencyInjector, AspectOrderStrategy);
        public IAspectDependencyInjector AspectDependencyInjector => new AspectDependencyInjector(DependencyProvider);
        public IAspectOrderStrategy AspectOrderStrategy => new DefaultAspectOrderStrategy();
    }
}