using System;
using System.Reflection;
using IvorySharp.Extensions;

namespace IvorySharp.Core
{
    /// <summary>
    /// Модель выполнения метода.
    /// </summary>
    internal class Invocation : IInvocation
    {
        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        private readonly Func<object, object[], object> _methodInvoker;
        
        /// <summary>
        /// Контекст выполнения метода.
        /// </summary>
        public InvocationContext Context { get; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        /// <param name="methodInvoker">Делегат для быстрого вызова метода.</param>
        internal Invocation(InvocationContext context, Func<object, object[], object> methodInvoker)
        {
            Context = context;
            _methodInvoker = methodInvoker;
        }

        /// <inheritdoc />
        public object Proceed()
        {
            try
            {
                Context.ReturnValue = _methodInvoker(Context.Instance, (object[]) Context.Arguments);

                if (ReferenceEquals(Context.ReturnValue, Context.Instance))
                    Context.ReturnValue = Context.TransparentProxy;

                return Context.ReturnValue;
            }
            catch (TargetInvocationException e)
            {
                e.Unwrap().Rethrow();
                throw;
            }
        }
    }
}