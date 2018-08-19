using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using IvorySharp.Components;
using IvorySharp.Extensions;
using IvorySharp.Integration.CastleWindsor.Aspects.Weaving;

namespace IvorySharp.Integration.CastleWindsor.Aspects.Integration
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

            var weavePredicate = _components.AspectWeavePredicate.Get();

            foreach (var serviceType in handler.ComponentModel.Services)
            {
                if (!weavePredicate.IsWeaveable(serviceType, 
                        handler.ComponentModel.Implementation))
                    continue;

                var interceptorType = typeof(WeavedInterceptor<>)
                    .MakeGenericType(serviceType);
                   
                handler.ComponentModel.Interceptors.AddIfNotInCollection(
                    new InterceptorReference(interceptorType));
            }
        }
    }
}