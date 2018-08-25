using System;
using System.Collections.Generic;
using System.Linq;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Фабрика провайдера данных о вызове.
    /// </summary>
    internal class InvocationWeaveDataProviderFactory : IInvocationWeaveDataProviderFactory
    {
        private readonly IComponentHolder<IAspectWeavePredicate> _weavePredicateHolder;
        private readonly IComponentHolder<IAspectFactory> _preInitializerHolder;
        private readonly IComponentHolder<IInvocationPipelineFactory> _pipelineFactoryHolder;
        private readonly IMethodInfoCache _methodInfoCache;
        
        private IAspectWeavePredicate _weavePredicate;
        private IAspectFactory _factory;
        private IInvocationPipelineFactory _pipelineFactory;

        private readonly IKeyValueCache<TypePair, IInvocationWeaveDataProvider> _providerCache;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationWeaveDataProviderFactory"/>.
        /// </summary>
        public InvocationWeaveDataProviderFactory(
            IComponentHolder<IAspectWeavePredicate> weavePredicateHolder, 
            IComponentHolder<IAspectFactory> preInitializerHolder, 
            IComponentHolder<IInvocationPipelineFactory> pipelineFactoryHolder,
            IMethodInfoCache methodInfoCache,
            IKeyValueCacheFactory cacheFactory)
        {
            _weavePredicateHolder = weavePredicateHolder;
            _preInitializerHolder = preInitializerHolder;
            _pipelineFactoryHolder = pipelineFactoryHolder;
            _methodInfoCache = methodInfoCache;
            _providerCache = cacheFactory.Create<TypePair, IInvocationWeaveDataProvider>();
        }

        /// <inheritdoc />
        public IInvocationWeaveDataProvider Create(Type declaredType, Type targetType)
        {
            var key = new TypePair(declaredType, targetType);

            return _providerCache.GetOrAdd(key, typeKey =>
            {
                var invocationsData = new Dictionary<IInvocationSignature, InvocationWeaveData>();

                var declaredMethods = typeKey.DeclaredType.GetMethods()
                    .Union(typeKey.DeclaredType.GetInterfaces()
                        .SelectMany(i => i.GetMethods()));

                foreach (var declaredMethod in declaredMethods)
                {
                    var targetMethod = _methodInfoCache.GetMethodMap(typeKey.TargetType, declaredMethod);
                    if (targetMethod == null)
                        throw new IvorySharpException(
                            $"Не удалось найти метод '{declaredMethod.Name}' в типе '{typeKey.TargetType.Name}'");

                    var methodInvoker = _methodInfoCache.GetInvoker(declaredMethod);

                    var signature = new InvocationSignature(
                        declaredMethod, targetMethod, typeKey.DeclaredType,
                        typeKey.TargetType, declaredMethod.GetInvocationType());

                    if (_weavePredicate == null)
                        _weavePredicate = _weavePredicateHolder.Get();

                    var isWeaveable = _weavePredicate.IsWeaveable(signature);
                    if (!isWeaveable)
                    {
                        invocationsData.Add(signature, InvocationWeaveData.Unweaveble(methodInvoker));
                        continue;
                    }

                    if (_factory == null)
                        _factory = _preInitializerHolder.Get();

                    var boundaryAspects = _factory.CreateBoundaryAspects(signature);
                    var interceptAspect = _factory.CreateInterceptAspect(signature);

                    if (_pipelineFactory == null)
                        _pipelineFactory = _pipelineFactoryHolder.Get();

                    var pipeline = _pipelineFactory.CreatePipeline(signature, boundaryAspects, interceptAspect);
                    var executor = _pipelineFactory.CreateExecutor(signature);

                    invocationsData.Add(signature,
                        InvocationWeaveData.Weaveble(
                            methodInvoker, pipeline, executor, boundaryAspects, interceptAspect));
                }

                return new InvocationWeaveDataProvider(invocationsData);
            });
        }
    }
}