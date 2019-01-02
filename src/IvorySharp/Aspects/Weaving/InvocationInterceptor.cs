using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using IvorySharp.Linq;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Класс, содержащий логику перехвата вызываемого метода.
    /// </summary>   
    internal sealed class InvocationInterceptor
    {
        private readonly IInvocationWeaveDataProvider _weaveDataProvider;
        private readonly IAspectDependencyInjector _dependencyInjector;
        private readonly IAspectFinalizer _aspectFinalizer;

        public InvocationInterceptor(
            IInvocationWeaveDataProvider weaveDataProvider,
            IComponentHolder<IAspectDependencyInjector> dependencyInjectorHolder,
            IComponentHolder<IAspectFinalizer> aspectFinalizerHolder)
        {
            _weaveDataProvider = weaveDataProvider;
            _dependencyInjector = dependencyInjectorHolder.Get();
            _aspectFinalizer = aspectFinalizerHolder.Get();
        }

        /// <summary>
        /// Выполняет перехват вызова исходного метода с применением аспектов.
        /// </summary>
        /// <param name="signature">Сигнатура метода.</param>
        /// <param name="methodCall">Делегат вызова метода.</param>
        /// <param name="args">Параметры вызова метода.</param>
        /// <param name="target">Экземпляр сервиса.</param>
        /// <param name="proxy">Прокси сервиса.</param>
        /// <returns>Результат вызова метода.</returns>
        internal object Intercept(
            IInvocationSignature signature, MethodCall methodCall,
            object[] args, object target, object proxy)
        {
            var invocationData = _weaveDataProvider.Get(signature);

            if (invocationData == null)
                throw new IvorySharpException(
                    $"Не найдена информация о вызываемом методе '{signature.Method.Name}'. " +
                    $"DeclaringType: {signature.DeclaringType.Name} " +
                    $"ImplementationType: {signature.TargetType.Name}");

            if (!invocationData.IsWeaveable)
                // Bypass
                return methodCall(target, args);

            var invocation = new Invocation(signature, args, proxy, target, methodCall);
            
            foreach (var aspect in invocationData.BoundaryAspects)
            {
                if (aspect.HasDependencies)
                    _dependencyInjector.InjectPropertyDependencies(aspect);         

                if (aspect.IsInitializable)
                    aspect.Initialize();
            }
            
            if (invocationData.InterceptionAspect.HasDependencies)
                _dependencyInjector.InjectPropertyDependencies(invocationData.InterceptionAspect);
            
            if (invocationData.InterceptionAspect.IsInitializable)
                invocationData.InterceptionAspect.Initialize();

            invocationData.PipelineExecutor.ExecutePipeline(invocationData.Pipeline, invocation);

            foreach (var aspect in invocationData.BoundaryAspects)
            {
                if (aspect.IsFinalizable)
                    _aspectFinalizer.Finalize(aspect);
            }

            if (invocationData.InterceptionAspect.IsFinalizable)
                _aspectFinalizer.Finalize(invocationData.InterceptionAspect);

            return invocation.ReturnValue;
        }
    }
}