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
    public class Invocation : IInvocation
    {
        /// <summary>
        /// Делегат для быстрого вызова метода.
        /// </summary>
        private readonly Func<object, object[], object> _methodInvoker;

        /// <inheritdoc />
        public InvocationContext Context { get; }

        /// <inheritdoc />
        public object ReturnValue
        {
            get => _returnValue;
            set => _returnValue = ConvertReturnValue(value);
        }
        
        private object _returnValue;
        
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
        public virtual object Proceed()
        {
            try
            {
                ReturnValue = _methodInvoker != null 
                    ? _methodInvoker(Context.Instance, (object[]) Context.Arguments) 
                    : Context.Method.Invoke(Context.Instance, (object[]) Context.Arguments);
                
                if (ReferenceEquals(ReturnValue, Context.Instance))
                    ReturnValue = Context.TransparentProxy;

                return ReturnValue;
            }
            catch (TargetInvocationException e)
            {
                e.GetInner().Throw();
                throw;
            }
        }
        
        /// <summary>
        /// Конвертирует возвращаемое значение вызова.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        public object ConvertReturnValue(object returnValue)
        {
            if (Context.Method.IsVoidReturn())
            {
                throw new IvorySharpException(
                    $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                    $"Метод '{Context.Method.Name}' типа '{Context.DeclaringType.FullName}' " +
                    "не имеет возвращаемого значения (void).");
            }

            if (returnValue == null)
                return Context.Method.ReturnType.GetDefaultValue();
            

            if (!TypeConversion.TryConvert(returnValue, Context.Method.ReturnType, out var converted))
            {
                throw new IvorySharpException(
                    $"Невозможно установить возвращаемое значение '{returnValue}'. " +
                    $"Тип результата '{returnValue.GetType().FullName}' " +
                    $"невозможно привести к возвращаемому типу '{Context.Method.ReturnType.FullName}'.");
            }

            return converted;
        }
    }
}