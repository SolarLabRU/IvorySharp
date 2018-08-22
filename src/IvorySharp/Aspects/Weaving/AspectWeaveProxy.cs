using System;
using System.ComponentModel;
using System.Reflection;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Proxying;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Прокси, связанное с аспектами.
    /// </summary>
    [PublicAPI, EditorBrowsable(EditorBrowsableState.Never)]
    public class AspectWeaveProxy : IvoryProxy
    {
        private IComponentHolder<IAspectFactory> _aspectFactory;
        private IComponentHolder<IInvocationPipelineFactory> _pipelineFactory;
        private IComponentHolder<IAspectWeavePredicate> _weavePredicate;
        private IMethodCache _methodCache;
        
        /// <summary>
        /// Исходный объект, вызовы которого будут перехватываться.
        /// </summary>
        internal object Target { get; private set; }

        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        internal object Proxy { get; private set; }

        /// <summary>
        /// Тип, в котором объявлен целевой метод (интерфейс).
        /// </summary>
        internal Type DeclaringType { get; private set; }

        /// <summary>
        /// Тип, в котором содержится реализация целевого метода.
        /// </summary>
        internal Type TargetType { get; private set; }
      
        /// <summary>
        /// Создает экземпляр прокси.
        /// </summary>
        /// <param name="target">Целевой объект</param>
        /// <param name="targetType">Тип целевого объекта.</param>
        /// <param name="declaringType">Тип интерфейса, реализуемого целевым классом.</param>
        /// <param name="aspectFactoryHolder">Фабрика аспектов.</param>
        /// <param name="pipelineFactoryHolder">Фабрика компонентов пайлпайна.</param>
        /// <param name="weavePredicateHolder">Предикат определения возможности применения аспектов.</param>
        /// <returns>Экземпляр прокси.</returns>
        internal static object Create(
            object target,
            Type targetType,
            Type declaringType,
            IComponentHolder<IAspectFactory> aspectFactoryHolder,
            IComponentHolder<IInvocationPipelineFactory> pipelineFactoryHolder,
            IComponentHolder<IAspectWeavePredicate> weavePredicateHolder)
        {
            var transparentProxy = ProxyGenerator.Instance.CreateTransparentProxy(
                typeof(AspectWeaveProxy), declaringType);
            
            var weavedProxy = (AspectWeaveProxy) transparentProxy;

            weavedProxy.Initialize(
                target, 
                transparentProxy,
                targetType,
                declaringType,
                aspectFactoryHolder, 
                pipelineFactoryHolder,
                weavePredicateHolder,
                MethodCache.Instance);

            return transparentProxy;
        }

        /// <inheritdoc />
        protected internal sealed override object Invoke(MethodInfo method, object[] args)
        {
            var invoker = _methodCache.GetInvoker(method);
            var invocation = new Invocation(args, method, DeclaringType, TargetType, Proxy, Target, invoker);     
            var interceptor = new InvocationInterceptor(_aspectFactory, _pipelineFactory, _weavePredicate);

            return interceptor.Intercept(invocation);
        }

        /// <summary>
        /// Выполняет инициализацию прокси.
        /// </summary>
        private void Initialize(
            object target,
            object proxy,
            Type targetType,
            Type declaringType,
            IComponentHolder<IAspectFactory> aspectFactoryHolder,
            IComponentHolder<IInvocationPipelineFactory> pipelineFactoryHolder,
            IComponentHolder<IAspectWeavePredicate> weavePredicateHolder,
            IMethodCache methodCache)
        {
            _aspectFactory = aspectFactoryHolder;
            _pipelineFactory = pipelineFactoryHolder;
            _weavePredicate = weavePredicateHolder;
            _methodCache = methodCache;

            Target = target;
            Proxy = proxy;
            TargetType = targetType;
            DeclaringType = declaringType;
        }
    }
}