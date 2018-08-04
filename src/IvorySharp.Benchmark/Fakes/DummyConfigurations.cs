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
        public IMethodAspectSelectionStrategy AspectSelectionStrategy => new DefaultMethodAspectSelectionStrategy();
        public IMethodAspectWeavePredicate AspectWeavePredicate => new DeclaringTypeWeavePredicate(AspectSelectionStrategy);
        public IMethodAspectDeclarationCollector AspectDeclarationCollector => new DeclaringTypeAspectDeclarationCollector(AspectSelectionStrategy);
        public IMethodAspectPipelineExecutor AspectPipelineExecutor => MethodAspectInvocationPipelineExecutor.Instance;
        public IMethodAspectInitializer AspectInitializer => new MethodAspectInitializer(AspectDeclarationCollector, AspectDependencyInjector, AspectOrderStrategy);
        public IMethodAspectDependencyInjector AspectDependencyInjector => new MethodAspectDependencyInjector(DependencyProvider);
        public IMethodAspectOrderStrategy AspectOrderStrategy => new DefaultMethodAspectOrderStrategy();
    }
}