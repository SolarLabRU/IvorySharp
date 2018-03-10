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
            get
            {
                return Context.ReturnValue;
            }
            set
            {
                Context.ReturnValue = value;
            }
        }

        /// <inheritdoc />
        public Type TargetType { get; }

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
                // TargetType не подходит, так как там - тип класса,
                // а не интерфейса
                Method.DeclaringType);
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
        }

        /// <inheritdoc />
        void IvorySharp.Core.IInvocation.Proceed()
        {
            ((IInvocation)this).Proceed();
            ReturnValue = _castleInvocation.ReturnValue;
        }
    }
}