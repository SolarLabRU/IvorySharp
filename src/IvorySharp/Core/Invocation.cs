using System;
using System.Reflection;
using IvorySharp.Exceptions;
using IvorySharp.Extensions;
using IvorySharp.Reflection;

namespace IvorySharp.Core
{
    /// <summary>
    /// Модель выполнения метода.
    /// </summary>
    public class Invocation : IInterceptableInvocation
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

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="Invocation"/>.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        internal Invocation(InvocationContext context)
        {
            Context = context;
        }
        
        /// <inheritdoc />
        public object Proceed()
        {
            try
            {
                Context.ReturnValue = _methodInvoker != null 
                    ? _methodInvoker(Context.Instance, (object[]) Context.Arguments) 
                    : Context.Method.Invoke(Context.Instance, (object[]) Context.Arguments);
                
                if (ReferenceEquals(Context.ReturnValue, Context.Instance))
                    Context.ReturnValue = Context.TransparentProxy;

                return Context.ReturnValue;
            }
            catch (TargetInvocationException e)
            {
                e.GetInner().Throw();
                throw;
            }
        }
        
        /// <summary>
        /// Устанавливает возвращаемое значение вызова.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        public void SetReturnValue(object returnValue)
        {
            if (Context.Method.IsVoidReturn())
            {
                throw new IvorySharpException(
                    $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                    $"Метод '{Context.Method.Name}' типа '{Context.DeclaringType.FullName}' " +
                    $"не имеет возвращаемого значения (void).");
            }

            if (returnValue == null)
            {
                Context.ReturnValue = Context.Method.ReturnType.GetDefaultValue();
            }
            else
            {
                if (!TypeConversion.TryConvert(returnValue, Context.Method.ReturnType, out var converted))
                {
                    throw new IvorySharpException(
                        $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                        $"Тип результата '{returnValue.GetType().FullName}' " +
                        $"невозможно привести к возвращаемому типу '{Context.Method.ReturnType.FullName}'.");
                }

                Context.ReturnValue = converted;
            }
        }
    }
}