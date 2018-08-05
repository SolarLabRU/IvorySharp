using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using IvorySharp.Aspects.Configuration;
using IvorySharp.CastleWindsor.Aspects.Weaving;
using IvorySharp.Extensions;

namespace IvorySharp.CastleWindsor.Aspects.Integration
{
    /// <summary>
    /// Объект настройки контейнера <see cref="IKernel"/>.
    /// </summary>
    internal class WindsorAspectFacility : AbstractFacility
    {
        private readonly IComponentsStore _components;

        /// <summary>
        /// Инициализирует экземпляр <see cref="WindsorAspectFacility"/>
        /// </summary>
        /// <param name="components">Компоненты библиотеки.</param>
        public WindsorAspectFacility(IComponentsStore components)
        {
            _components = components;
        }

        /// <inheritdoc />
        protected override void Init()
        {
            Kernel.ComponentRegistered += OnComponentRegistered;
        }

        private void OnComponentRegistered(string key, IHandler handler)
        {
            var componentInterfaces = handler.ComponentModel.Implementation.GetInterfaces();
            if (componentInterfaces.Any(i => !i.IsInterceptable()))
                return;

            foreach (var serviceType in handler.ComponentModel.Services)
            {
                if (!_components.AspectWeavePredicate.IsWeaveable(serviceType, handler.ComponentModel.Implementation))
                    continue;
                
                handler.ComponentModel.Interceptors.AddIfNotInCollection(
                    new InterceptorReference(typeof(WeavedInterceptor)));
            }
        }
    }
}