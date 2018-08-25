using System.Collections.Generic;
using IvorySharp.Core;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Провайдер данных <see cref="InvocationWeaveData"/>.
    /// </summary>
    internal class InvocationWeaveDataProvider : IInvocationWeaveDataProvider
    {
        private readonly IDictionary<IInvocationSignature, InvocationWeaveData> _data;

        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationWeaveDataProvider"/>.
        /// </summary>
        /// <param name="data">Данные о вызовах.</param>
        public InvocationWeaveDataProvider(IDictionary<IInvocationSignature, InvocationWeaveData> data)
        {
            _data = data;
        }

        /// <inheritdoc />
        public InvocationWeaveData Get(IInvocationSignature signature)
        {
            return _data.TryGetValue(signature, out var data) ? data : null;
        }
    }
}