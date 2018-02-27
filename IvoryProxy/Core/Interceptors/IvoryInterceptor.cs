using System;

namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Базовый класс перехватчика вызовов.
    /// </summary>
    public abstract class IvoryInterceptor : IInterceptor
    {
        private string _inverceptorKey;

        /// <summary>
        /// Инициализирует экземпляр <see cref="IvoryInterceptor"/>.
        /// </summary>
        protected IvoryInterceptor()
        {
            _inverceptorKey = GetType().FullName;
        }

        /// <inheritdoc />
        public virtual string InterceptorKey
        {
            get => _inverceptorKey;
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Ключ перехватчика должен отличаться от пустой строки",
                        nameof(InterceptorKey));
                }

                _inverceptorKey = value;
            }
        }

        /// <inheritdoc />
        public abstract void Intercept(IInvocation invocation);
    }
}