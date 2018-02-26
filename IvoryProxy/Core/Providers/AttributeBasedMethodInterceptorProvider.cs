using System;
using System.Reflection;
using IvoryProxy.Core.Activators;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Core.Providers
{
    /// <summary>
    /// Провайдер перехватчиков вызовов методов на основе атрибутов.
    /// </summary>
    internal class AttributeBasedMethodInterceptorProvider : IMethodInterceptorProvider
    {
        /// <summary>
        /// Активатор перехватчиков.
        /// </summary>
        protected IMethodInterceptorActivator MethodInterceptorActivator { get; set; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeBasedMethodInterceptorProvider"/>
        /// с активатором по умолчанию <see cref="DefaultCtorMethodInterceptorActivator"/>.
        /// </summary>
        public AttributeBasedMethodInterceptorProvider()
        {
            MethodInterceptorActivator = new DefaultCtorMethodInterceptorActivator();
        }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="AttributeBasedMethodInterceptorProvider"/>.
        /// </summary>
        /// <param name="activator">Активатор перехватчиков вызовов методов.</param>
        public AttributeBasedMethodInterceptorProvider(IMethodInterceptorActivator activator)
        {
            if (activator == null)
                throw new ArgumentNullException(nameof(activator));
            
            MethodInterceptorActivator = activator;
        }
        
        /// <inheritdoc />
        public IMethodInterceptor GetInterceptor(IMethodInvocation invocation)
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            var interceptionAttribute = invocation.TargetMethod.GetCustomAttribute<InterceptMethodAttribute>();
            if (interceptionAttribute == null)
                return BypassMethodInterceptor.Instance;

            return MethodInterceptorActivator.CreateInstance(interceptionAttribute.InterceptorType);
        }
    }
}