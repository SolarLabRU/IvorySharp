using System;
using IvorySharp.Caching;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент для предварительной подготовки аспектов к инициализации с кешем.
    /// </summary>
    internal class CachedAspectsPreInitializer : IAspectsPreInitializer
    {
        private readonly IAspectsPreInitializer _originalPreInitializer;
        
        private readonly Func<IInvocationContext, MethodBoundaryAspect[]> _cachedPrepareMethodBoundaryAspect;
        private readonly Func<IInvocationContext, MethodInterceptionAspect> _cachedPrepareMethodInterceptionAspect;

        /// <summary>
        /// Инициализирует экземпляр <see cref="CachedAspectsPreInitializer"/>.
        /// </summary>
        public CachedAspectsPreInitializer(IAspectsPreInitializer originalPreInitializer)
        {
            _originalPreInitializer = originalPreInitializer;

            _cachedPrepareMethodBoundaryAspect = Memoizer.CreateProducer(
                ctx => _originalPreInitializer.PrepareBoundaryAspects(ctx),
                InvocationContextComparer.Instance);

            _cachedPrepareMethodInterceptionAspect = Memoizer.CreateProducer(
                ctx => _originalPreInitializer.PrepareInterceptAspect(ctx),
                InvocationContextComparer.Instance);
        }

        /// <inheritdoc />
        public MethodBoundaryAspect[] PrepareBoundaryAspects(IInvocationContext context)
        {
            return _cachedPrepareMethodBoundaryAspect(context);
        }

        /// <inheritdoc />
        public MethodInterceptionAspect PrepareInterceptAspect(IInvocationContext context)
        {
            return _cachedPrepareMethodInterceptionAspect(context);
        }
    }
}