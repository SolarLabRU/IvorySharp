using System;
using IvorySharp.Proxying;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    public class AspectWeaver
    {
        private readonly IInterceptProxyGenerator _proxyGenerator;
        private readonly IMethodAspectSelector _methodAspectSelector;

        public AspectWeaver(IInterceptProxyGenerator proxyGenerator, IMethodAspectSelector methodAspectSelector)
        {
            _proxyGenerator = proxyGenerator;
            _methodAspectSelector = methodAspectSelector;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="targetDeclaredType">Объявленный тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="targetDeclaredType"/>.</returns>
        public object Weave(object target, Type targetDeclaredType)
        {
            var interceptor = new AspectWeaveInterceptor(_methodAspectSelector);
            return _proxyGenerator.CreateInterceptProxy(target, targetDeclaredType, interceptor);
        }
    }
}