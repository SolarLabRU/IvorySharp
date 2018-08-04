using System;
using IvorySharp.Aspects.Components.Creation;
using IvorySharp.Aspects.Components.Dependency;
using IvorySharp.Aspects.Components.Selection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Tests.WeavingSettings
{
    public class NullComponentsStore : IComponentsStore
    {
        public IDependencyProvider DependencyProvider { get; } = null;      
        public IMethodAspectSelectionStrategy AspectSelectionStrategy => new DefaultMethodAspectSelectionStrategy();
        public IMethodAspectWeavePredicate AspectWeavePredicate => new DeclaringTypeWeavePredicate(AspectSelectionStrategy);
        public IMethodAspectDeclarationCollector AspectDeclarationCollector => new DeclaringTypeAspectDeclarationCollector(AspectSelectionStrategy);
        public IMethodAspectPipelineExecutor AspectPipelineExecutor => MethodAspectInvocationPipelineExecutor.Instance;
        public IMethodAspectInitializer AspectInitializer => new MethodAspectInitializer(AspectDeclarationCollector, AspectDependencyInjector, AspectOrderStrategy);
        public IMethodAspectDependencyInjector AspectDependencyInjector => new MethodAspectDependencyInjector(DependencyProvider);
        public IMethodAspectOrderStrategy AspectOrderStrategy => new DefaultMethodAspectOrderStrategy();
    }
}