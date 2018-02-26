using System;
using System.Reflection;
using IvoryProxy.Core.Activators;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Extensions;

namespace IvoryProxy.Core.Providers
{
    /// <summary>
    /// Провайдер перехватчиков вызовов методов на основе атрибутов.
    /// </summary>
    internal class AttributeBasedInterceptorProvider : IInterceptorProvider
    {
        /// <summary>
        /// Активатор перехватчиков.
        /// </summary>
        protected IInterceptorActivator InterceptorActivator { get; set; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeBasedInterceptorProvider"/>
        /// с активатором по умолчанию <see cref="DefaultCtorInterceptorActivator"/>.
        /// </summary>
        public AttributeBasedInterceptorProvider()
        {
            InterceptorActivator = new DefaultCtorInterceptorActivator();
        }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeBasedInterceptorProvider"/>.
        /// </summary>
        /// <param name="activator">Активатор перехватчиков вызовов методов.</param>
        public AttributeBasedInterceptorProvider(IInterceptorActivator activator)
        {
            if (activator == null)
                throw new ArgumentNullException(nameof(activator));
            
            InterceptorActivator = activator;
        }
        
        /// <inheritdoc />
        public IInterceptor GetInterceptor(IMethodInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (invocation.IsInterceptDisallowed())
                return null;

            var interceptorAttribute = invocation.TargetMethod.GetCustomAttribute<InterceptAttribute>();
            if (interceptorAttribute == null)
                interceptorAttribute = invocation.DeclaringType.GetCustomAttribute<InterceptAttribute>();

            return interceptorAttribute == null 
                ? null : InterceptorActivator.CreateInstance(interceptorAttribute.InterceptorType);
        }
    }
}