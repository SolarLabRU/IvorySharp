using System;
using System.Linq;
using System.Reflection;
using IvoryProxy.Core.Activators;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Core.Interceptors;
using IvoryProxy.Extensions;

namespace IvoryProxy.Core.Providers
{
    /// <summary>
    /// Провайдер перехватчиков вызовов методов на основе атрибутов.
    /// </summary>
    internal class AttributeInterceptorProvider : IInterceptorProvider
    {
        /// <summary>
        /// Активатор перехватчиков.
        /// </summary>
        protected IInterceptorActivator InterceptorActivator { get; set; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeInterceptorProvider"/>
        /// с активатором по умолчанию <see cref="DefaultCtorInterceptorActivator"/>.
        /// </summary>
        public AttributeInterceptorProvider()
        {
            InterceptorActivator = new DefaultCtorInterceptorActivator();
        }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeInterceptorProvider"/>.
        /// </summary>
        /// <param name="activator">Активатор перехватчиков вызовов методов.</param>
        public AttributeInterceptorProvider(IInterceptorActivator activator)
        {
            if (activator == null)
                throw new ArgumentNullException(nameof(activator));
            
            InterceptorActivator = activator;
        }

        /// <inheritdoc />
        public IInterceptor GetInterceptor(IInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (invocation.IsInterceptionDisallowed())
                return new BypassInterceptor();

            var bindings = CreateBindings(invocation);
            if (bindings.Length == 0)
                return new BypassInterceptor();

            var interceptors = Array.ConvertAll(bindings, b => InterceptorActivator.CreateInstance(b.InterceptorType))
                .DistinctBy(a => a.InterceptorKey)
                .ToArray();
            
            return new MasterInterceptor(interceptors);
        }

        private InterceptorBinding[] CreateBindings(IInvocation invocation)
        {
            var methodAttributes = invocation.TargetMethod.GetCustomAttributes<InterceptAttribute>() 
                                   ?? Array.Empty<InterceptAttribute>();
            
            var interfaceAttributes = invocation.DeclaringType.GetCustomAttributes<InterceptAttribute>() 
                                      ?? Array.Empty<InterceptAttribute>();

            return methodAttributes
                .Select(
                    ma => new InterceptorBinding(ma.GetType(), ma.InterceptorType,
                        InterceptorAttributePlacement.Method))
                .Concat(
                    interfaceAttributes
                        .Select(
                            ia => new InterceptorBinding(ia.GetType(), ia.InterceptorType,
                                InterceptorAttributePlacement.Interface)))
                .OrderByDescending(b => (int) b.AttributePlacement)
                .ToArray();
        }
    }
}