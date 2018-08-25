using System;
using IvorySharp.Components;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Фабрика провайдера данных о вызове.
    /// </summary>
    [NotNull]
    public interface IInvocationWeaveDataProviderFactory : IComponent
    {
        /// <summary>
        /// Создает экземпляр <see cref="IInvocationWeaveDataProvider"/>.
        /// </summary>
        /// <param name="declaredType">Объявленный тип компонента.</param>
        /// <param name="targetType">Фактический тип компонента.</param>
        /// <returns>Экземпляр <see cref="IInvocationWeaveDataProvider"/>.</returns>
        [NotNull] IInvocationWeaveDataProvider Create([NotNull] Type declaredType, [NotNull] Type targetType);
    }
}