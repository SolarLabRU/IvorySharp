using System;
using System.Linq;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Integration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using Microsoft.Extensions.DependencyInjection;

namespace IvorySharp.Integration.Microsoft.DependencyInjection.Aspects.Integration
{
    /// <summary>
    /// Контейнер аспектов.
    /// </summary>
    public class MicrosoftAspectContainer : AspectsContainer
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IDependencyProvider _dependencyProvider;

        internal MicrosoftAspectContainer(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _dependencyProvider = new MicrosoftDependencyProvider(serviceCollection);
        }
        
        /// <inheritdoc />
        public override void BindAspects(IComponentsStore settings)
        {
            var weaver = AspectWeaverFactory.Create(settings);
            var predicate = settings.AspectWeavePredicate.Get();

            var validDescriptors = _serviceCollection
                .Select((d, i) => new { Descriptor = d, Index = i})
                .Where(o => predicate.IsWeaveable(o.Descriptor.ServiceType, o.Descriptor.ImplementationType))
                .ToArray();
            
            foreach (var packed in validDescriptors)
            {
                var descriptor = packed.Descriptor;     
                if (descriptor.ServiceType.IsGenericTypeDefinition)
                {
                    throw new NotSupportedException(
                        $"Регистрация open-generic не поддерживается (serviceType: '{descriptor.ServiceType.Name}')");
                }
                
                var replace = ServiceDescriptor.Describe(
                    descriptor.ServiceType, 
                    provider => weaver.Weave(
                        GetServiceInstance(provider, descriptor), 
                        descriptor.ServiceType, 
                        descriptor.ImplementationType),
                    descriptor.Lifetime);
               
                _serviceCollection.Insert(packed.Index, replace);
                _serviceCollection.Remove(packed.Descriptor);
            }
        }

        /// <inheritdoc />
        public override IDependencyProvider GetDependencyProvider()
        {
            return _dependencyProvider;
        }

        private static object GetServiceInstance(IServiceProvider serviceProvider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
                return descriptor.ImplementationInstance;

            return descriptor.ImplementationType != null 
                ? ActivatorUtilities.GetServiceOrCreateInstance(
                    serviceProvider, descriptor.ImplementationType) 
                : descriptor.ImplementationFactory(serviceProvider);
        }
    }
}