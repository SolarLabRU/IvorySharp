using IvorySharp.Core;
using JetBrains.Annotations;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Провайдер данных о вызове.
    /// </summary>
    [PublicAPI]
    public interface IInvocationWeaveDataProvider
    {
        /// <summary>
        /// Получает данные о вызове.
        /// </summary>
        /// <param name="signature">Сигнатура вызова.</param>
        /// <returns>Данные о вызове.</returns>
        [NotNull] InvocationWeaveData Get([NotNull] IInvocationSignature signature);
    }
}