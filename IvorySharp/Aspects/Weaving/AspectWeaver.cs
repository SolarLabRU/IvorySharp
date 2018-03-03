using System;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Proxying;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    public class AspectWeaver
    {
        private readonly IWeavingAspectsConfiguration _configuration;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <param name="configuration">Конфигурация аспектов.</param>
        public AspectWeaver(IWeavingAspectsConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="targetDeclaredType">Объявленный тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="targetDeclaredType"/>.</returns>
        public object Weave(object target, Type targetDeclaredType)
        {
            var interceptor = new AspectWeaveInterceptor(_configuration);
            return InterceptProxyGenerator.Default.CreateInterceptProxy(target, targetDeclaredType, interceptor);
        }
    }
}