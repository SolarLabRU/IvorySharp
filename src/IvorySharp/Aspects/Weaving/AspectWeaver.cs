using System;
using System.Runtime.CompilerServices;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Exceptions;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    [PublicAPI]
    public sealed class AspectWeaver
    {
        private readonly IComponentHolder<IAspectDependencyInjector> _aspectDependencyInjectorHolder;
        private readonly IComponentHolder<IAspectFinalizer> _aspectFinalizerHolder;
        private readonly IComponentHolder<IInvocationWeaveDataProviderFactory> _weaveDataProviderFactoryHolder;

        private IInvocationWeaveDataProviderFactory _weaveDataProviderFactory;
        
        /// <summary>
        /// Инициализирует экезмпляр <see cref="AspectWeaver"/>.
        /// </summary>
        internal AspectWeaver(
            IComponentHolder<IInvocationWeaveDataProviderFactory> weaveDataProviderFactoryHolder,
            IComponentHolder<IAspectDependencyInjector> aspectDependencyInjectorHolder,
            IComponentHolder<IAspectFinalizer> aspectFinalizerHolder)
        {
            _weaveDataProviderFactoryHolder = weaveDataProviderFactoryHolder;
            _aspectDependencyInjectorHolder = aspectDependencyInjectorHolder;
            _aspectFinalizerHolder = aspectFinalizerHolder;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="declaringType">Объявленный тип исходного объекта.</param>
        /// <param name="targetType">Фактический тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="declaringType"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull] public object Weave([NotNull] object target, [NotNull] Type declaringType, [NotNull] Type targetType)
        {
            if (_weaveDataProviderFactory == null)
                _weaveDataProviderFactory = _weaveDataProviderFactoryHolder.Get();
            
            var dataProvider = _weaveDataProviderFactory.Create(declaringType, targetType);

            return AspectWeaveProxy.Create(
                target,
                targetType, 
                declaringType,
                dataProvider,
                _aspectDependencyInjectorHolder,
                _aspectFinalizerHolder,
                MethodInfoCache.Instance);
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <typeparam name="TService">Объявленный тип.</typeparam>
        /// <typeparam name="TImplementation">Фактический тип.</typeparam>
        /// <returns>Экземпляр сервиса связанного с аспектами.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull] public TService Weave<TService, TImplementation>([NotNull] TImplementation target)
            where TImplementation : TService
        {
            if (!typeof(TService).IsInterface)
                throw new IvorySharpException($"Тип '{typeof(TService).Name}' не является интерфейсом");

            return (TService)Weave(target, typeof(TService), typeof(TImplementation));
        }
    }
}