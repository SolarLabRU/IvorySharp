using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.CastleWindsor.Aspects.Weaving;
using IvorySharp.Core;

namespace IvorySharp.CastleWindsor.Aspects.Integration
{
    public class WindsorAspectFacility : AbstractFacility
    {
        private readonly IWeavingAspectsConfiguration _configurations;

        private static Type[] _notWeavableTypes = new[]
        {
            typeof(IMethodAspect),
            typeof(IMethodBoundaryAspect),
            typeof(IInterceptor)
        };

        public WindsorAspectFacility(IWeavingAspectsConfiguration configurations)
        {
            _configurations = configurations;
        }

        /// <inheritdoc />
        protected override void Init()
        {
            Kernel.ComponentRegistered += OnComponentRegistered;
        }

        private void OnComponentRegistered(string key, IHandler handler)
        {
            var componentInterfaces = handler.ComponentModel.Implementation.GetInterfaces();
            if (componentInterfaces.Any(i => _notWeavableTypes.Contains(i)))
                return;

            var hasExplicitAttribute = _configurations.ExplicitWeaingAttributeType != null;
            foreach (var serviceType in handler.ComponentModel.Services)
            {
                if (hasExplicitAttribute)
                {
                    var attribute = serviceType.GetCustomAttribute(_configurations.ExplicitWeaingAttributeType);
                    if (attribute == null)
                        continue;
                }
                
                handler.ComponentModel.Interceptors.AddIfNotInCollection(
                    new InterceptorReference(typeof(AspectWeaverInterceptorAdapter)));
            }
        }
    }
}