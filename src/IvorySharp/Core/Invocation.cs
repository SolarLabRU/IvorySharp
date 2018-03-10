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
        /// Контекст выполнения метода.
        /// </summary>
        public InvocationContext Context { get; }

        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        public Func<object, object[], object> MethodInvoker { get; }

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        /// <param name="methodInvoker">Делегат для быстрого вызова метода.</param>
        internal Invocation(InvocationContext context, Func<object, object[], object> methodInvoker)
        {
            Context = context;
            MethodInvoker = methodInvoker;
        }

        /// <inheritdoc />
        public void Proceed()
        {
            try
            {
                Context.ReturnValue = MethodInvoker(Context.Instance, (object[]) Context.Arguments);
            }
            catch (TargetInvocationException e)
            {
                e.Unwrap().Rethrow();
            }
        }
    }
}