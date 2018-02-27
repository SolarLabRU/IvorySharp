using System;
using System.Collections.Generic;

namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Корневой обработчик.
    /// </summary>
    internal class MasterInterceptor : IInterceptor
    {
        /// <summary>
        /// Коллекция дочерних перехватчиков.
        /// </summary>
        public IReadOnlyCollection<IInterceptor> Interceptors { get; }

        /// <inheritdoc />
        public string InterceptorKey { get; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="MasterInterceptor"/>.
        /// </summary>
        /// <param name="interceptors">Массив перехватчиков.</param>
        public MasterInterceptor(IInterceptor[] interceptors)
        {
            Interceptors = interceptors ?? Array.Empty<IInterceptor>();
            InterceptorKey = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            if (Interceptors.Count == 0)
            {
                invocation.Proceed();
            }
            else
            {
                Action<IInvocation> invocationProceeder = _ => { };

                foreach (var interceptor in Interceptors)
                {
                    var proceeder = invocationProceeder;
                    invocationProceeder = (i) => { interceptor.Intercept(ApplyProceeder(proceeder)); };
                }

                invocationProceeder(invocation);

            }

            IInvocation ApplyProceeder(Action<IInvocation> proceeder)
            {
                proceeder(invocation);
                return invocation;
            }
        }
    }
}