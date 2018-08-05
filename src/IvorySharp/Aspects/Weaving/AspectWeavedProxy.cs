using System;
using System.Reflection;
using IvorySharp.Aspects.Caching;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Core;
using IvorySharp.Proxying;
using IvorySharp.Reflection;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Прокси, связанное с аспектами.
    /// </summary>
    public class AspectWeavedProxy : DispatchProxy
    {
        private IAspectFactory _aspectFactory;
        private IPipelineExecutor _pipelineExecutor;
        private IAspectWeavePredicate _weavePredicate;
        private Func<MethodInfo, Func<object, object[], object>> _methodInvokerFactory;

        /// <summary>
        /// Исходный объект, вызовы которого будут перехватываться.
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        public object Proxy { get; private set; }

        /// <summary>
        /// Тип, в котором объявлен целевой метод (интерфейс).
        /// </summary>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Тип, в котором содержится реализация целевого метода.
        /// </summary>
        public Type TargetType { get; private set; }
      
        /// <summary>
        /// Создает экземпляр прокси.
        /// </summary>
        /// <param name="target">Целевой объект</param>
        /// <param name="targetType">Тип целевого объекта.</param>
        /// <param name="declaringType">Тип интерфейса, реализуемого целевым классом.</param>
        /// <param name="aspectFactory">Фабрика аспектов.</param>
        /// <param name="pipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="weavePredicate">Предикат определения возможности применения аспектов.</param>
        /// <returns>Экземпляр прокси.</returns>
        public static object Create(
            object target,
            Type targetType,
            Type declaringType,
            IAspectFactory aspectFactory,
            IPipelineExecutor pipelineExecutor,
            IAspectWeavePredicate weavePredicate)
        {
            var transparentProxy = CreateTrasparentProxy<AspectWeavedProxy>(declaringType);
            var weavedProxy = (AspectWeavedProxy) transparentProxy;

            weavedProxy.Initialize(target, transparentProxy, targetType, declaringType, aspectFactory, pipelineExecutor, weavePredicate);

            return transparentProxy;
        }

        /// <inheritdoc />
        protected internal override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var context = new InvocationContext(args, targetMethod, Target, Proxy, DeclaringType, TargetType);
            var invocation = new Invocation(context, _methodInvokerFactory(targetMethod));
            var facade = new WeavedInvocationInterceptor(_aspectFactory, _pipelineExecutor, _weavePredicate);

            return facade.InterceptInvocation(invocation);
        }

        /// <summary>
        /// Выполняет инициализацию класса.
        /// </summary>
        /// <param name="target">Целевой объект</param>
        /// <param name="proxy">Экземпляр прокси.</param>
        /// <param name="targetType">Тип целевого объекта.</param>
        /// <param name="declaringType">Тип интерфейса, реализуемого целевым классом.</param>
        /// <param name="aspectFactory">Фабрика аспектов.</param>
        /// <param name="pipelineExecutor">Компонент выполнения пайплайна.</param>
        /// <param name="weavePredicate">Предикат определения возможности применения аспектов.</param>
        protected void Initialize(
            object target,
            object proxy,
            Type targetType,
            Type declaringType,
            IAspectFactory aspectFactory,
            IPipelineExecutor pipelineExecutor,
            IAspectWeavePredicate weavePredicate)
        {
            _methodInvokerFactory = Cache.CreateProducer<MethodInfo, Func<object, object[], object>>(
                Expressions.CreateMethodInvoker);
            
            _aspectFactory = aspectFactory;
            _pipelineExecutor = pipelineExecutor;
            _weavePredicate = weavePredicate;

            Target = target;
            Proxy = proxy;
            TargetType = targetType;
            DeclaringType = declaringType;
        }
    }
}