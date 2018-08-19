using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Caching;
using IvorySharp.Comparers;
using IvorySharp.Components;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент для предварительной подготовки аспектов с поддержкой кеша.
    /// </summary>
    /// <typeparam name="TAspect">Тип аспекта.</typeparam>
    internal class CachedAspectPreInitializer<TAspect> : IAspectPreInitializer<TAspect> 
        where TAspect : OrderableMethodAspect
    {
        private readonly IComponentProvider<IAspectPreInitializer<TAspect>> _preInitializerProvider;
        private Func<IInvocationContext, TAspect[]> _cachedPrepare;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedAspectPreInitializer{TAspect}"/>.
        /// </summary>
        public CachedAspectPreInitializer(
            IComponentProvider<IAspectPreInitializer<TAspect>> preInitializerProvider)
        {
            _preInitializerProvider = preInitializerProvider;
        }

        /// <inheritdoc />
        public TAspect[] PrepareAspects(IInvocationContext context)
        {
            if (_cachedPrepare == null)
            {
                _cachedPrepare = Memoizer.CreateProducer(
                    ctx => _preInitializerProvider.Get().PrepareAspects(ctx),
                    InvocationContextMethodComparer.Instance);
            }

            return _cachedPrepare(context);
        }
    }
}