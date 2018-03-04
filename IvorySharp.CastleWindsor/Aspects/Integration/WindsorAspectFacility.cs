using System;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.CastleWindsor.Aspects.Weaving;
using IvorySharp.Core;

namespace IvorySharp.CastleWindsor.Aspects.Integration
{
    public class WindsorAspectFacility : AbstractFacility
    {
        private readonly IAspectsWeavingSettings _settings;

        public WindsorAspectFacility(IAspectsWeavingSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        protected override void Init()
        {
            Kernel.ComponentRegistered += OnComponentRegistered;
        }

        private void OnComponentRegistered(string key, IHandler handler)
        {
            var componentInterfaces = handler.ComponentModel.Implementation.GetInterfaces();
            if (componentInterfaces.Any(i => AspectWeaver.NotWeavableTypes.Contains(i)))
                return;

            foreach (var serviceType in handler.ComponentModel.Services)
            {
                if (!AspectWeaver.IsWeavable(serviceType, _settings))
                    continue;
                
                handler.ComponentModel.Interceptors.AddIfNotInCollection(
                    new InterceptorReference(typeof(AspectWeaverInterceptorAdapter)));
            }
        }
    }
}