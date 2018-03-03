using System;
using System.Collections.Concurrent;
using System.Reflection;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Модель выполнения метода.
    /// </summary>
    internal class Invocation : IInvocation
    {
        private static ConcurrentDictionary<MethodInfo, Func<object, object[], object>> _invokersCache;

        static Invocation()
        {
            _invokersCache = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();
        }

        /// <summary>
        /// Контекст выполнения метода.
        /// </summary>
        public InvocationContext Context { get; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        internal Invocation(InvocationContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        public void Proceed()
        {
            if (!_invokersCache.TryGetValue(Context.Method, out var invoker))
            {
                invoker = Expressions.CreateMethodInvoker(Context.Method);
                _invokersCache.AddOrUpdate(Context.Method, _ => invoker, (_, __) => invoker);
            }

            try
            {
                Context.ReturnValue = invoker(Context.Instance, (object[]) Context.Arguments);
            }
            catch (TargetInvocationException e)
            {
                e.Unwrap().Rethrow();
            }
        }
    }
}