using System;
using System.Reflection;
using IvorySharp.Core;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace IvorySharp.CastleWindsor.Core
{
    /// <summary>
    /// Адаптирует модель вызова под модель вызова Castle.
    /// </summary>
    public class InvocationAdapter : IvorySharp.Core.IInvocation, IInvocation
    {
        private readonly IInvocation _castleInvocation;
            
        /// <inheritdoc />
        public InvocationContext Context { get; }

        /// <inheritdoc />
        public object[] Arguments { get; }

        /// <inheritdoc />
        public Type[] GenericArguments { get; }

        /// <inheritdoc />
        public object InvocationTarget { get; }

        /// <inheritdoc />
        public MethodInfo Method { get; }

        /// <inheritdoc />
        public MethodInfo MethodInvocationTarget { get; }

        /// <inheritdoc />
        public object Proxy { get; }

        /// <inheritdoc />
        public object ReturnValue
        {
            get => Context.ReturnValue;
            set => Context.ReturnValue = value;
        }

        /// <inheritdoc />
        public Type TargetType { get; }

        /// <summary>
        /// Адаптер для модели вызова Castle Windsor.
        /// </summary>
        /// <param name="castleInvocation">Модель вызова Castle Windsor.</param>
        public InvocationAdapter(IInvocation castleInvocation)
        {
            _castleInvocation = castleInvocation;
                
            Arguments = castleInvocation.Arguments;
            GenericArguments = castleInvocation.GenericArguments;
            InvocationTarget = castleInvocation.InvocationTarget;
            Method = castleInvocation.Method;
            MethodInvocationTarget = castleInvocation.MethodInvocationTarget;
            Proxy = castleInvocation.Proxy;
            TargetType = castleInvocation.TargetType;

            Context = new InvocationContext(
                castleInvocation.Arguments,
                castleInvocation.Method,
                InvocationTarget,
                castleInvocation.Proxy,
                Method.DeclaringType,
                castleInvocation.TargetType);
        }
            
        /// <inheritdoc />
        public object GetArgumentValue(int index)
        {
            return _castleInvocation.GetArgumentValue(index);
        }

        /// <inheritdoc />
        public MethodInfo GetConcreteMethod()
        {
            return _castleInvocation.GetConcreteMethod();
        }

        /// <inheritdoc />
        public MethodInfo GetConcreteMethodInvocationTarget()
        {
            return _castleInvocation.GetConcreteMethodInvocationTarget();
        }

        /// <inheritdoc />
        public void SetArgumentValue(int index, object value)
        {
            _castleInvocation.SetArgumentValue(index, value);
        }

        /// <inheritdoc />
        void IInvocation.Proceed()
        {
            _castleInvocation.Proceed();

            ReturnValue = _castleInvocation.ReturnValue;

            if (ReferenceEquals(ReturnValue, Context.Instance))
                ReturnValue = Proxy;
        }

        /// <inheritdoc />
        object IvorySharp.Core.IInvocation.Proceed()
        {
            var castleInvocation = ((Castle.DynamicProxy.IInvocation) this);
            castleInvocation.Proceed();
            return castleInvocation.ReturnValue;
        }
    }
}