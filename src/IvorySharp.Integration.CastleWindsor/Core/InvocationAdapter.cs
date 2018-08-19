using System;
using System.Reflection;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IInvocation = Castle.DynamicProxy.IInvocation;

namespace IvorySharp.Integration.CastleWindsor.Core
{
    /// <summary>
    /// Адаптирует модель вызова под модель вызова Castle.
    /// </summary>
    public class InvocationAdapter : IvorySharp.Core.IInvocation, IInvocation
    {
        private readonly IInvocation _castleInvocation;

        #region Castle
        
        /// <inheritdoc />
        public object[] Arguments => _castleInvocation.Arguments;

        /// <inheritdoc />
        public Type[] GenericArguments => _castleInvocation.GenericArguments;

        /// <inheritdoc />
        public object InvocationTarget => _castleInvocation.InvocationTarget;

        /// <inheritdoc />
        MethodInfo IInvocation.Method => _castleInvocation.Method;

        /// <inheritdoc />
        public MethodInfo MethodInvocationTarget => _castleInvocation.MethodInvocationTarget;

        /// <inheritdoc />
        object IInvocation.Proxy => _castleInvocation.Proxy;

        /// <inheritdoc />
        object IInvocation.ReturnValue
        {
            get => _castleInvocation.ReturnValue;
            set => _castleInvocation.ReturnValue = value;
        }

        /// <inheritdoc />
        Type IInvocation.TargetType => _castleInvocation.TargetType;

        /// <inheritdoc />
        public object GetArgumentValue(int index)
        {
            return _castleInvocation.GetArgumentValue(index);
        }
        
        /// <inheritdoc />
        public void SetArgumentValue(int index, object value)
        {
            _castleInvocation.SetArgumentValue(index, value);
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
        void IInvocation.Proceed()
        {
            _castleInvocation.Proceed();
        }       

        #endregion

        #region Ivory
        
        /// <inheritdoc />
        InvocationArguments IInvocationContext.Arguments => Arguments;

        /// <inheritdoc />
        public MethodInfo Method => _castleInvocation.Method;

        /// <inheritdoc />
        public MethodInfo TargetMethod => _targetMethodProvider.Value;
        private readonly Lazy<MethodInfo> _targetMethodProvider;
        
        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <inheritdoc />
        public Type TargetType => _castleInvocation.TargetType;

        /// <inheritdoc />
        public object Proxy => _castleInvocation.Proxy;

        /// <inheritdoc />
        public object Target => _castleInvocation.InvocationTarget;

        /// <inheritdoc />
        public InvocationType InvocationType { get; }

        /// <inheritdoc />
        public object ReturnValue
        {
            get => ((IInvocation) this).ReturnValue;
            set => ((IInvocation) this).ReturnValue = value;
        }

        /// <inheritdoc />
        public object Proceed()
        {
            ((IInvocation)this).Proceed();
            return ReturnValue;
        }
        
        #endregion
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="InvocationAdapter"/>.
        /// </summary>
        public InvocationAdapter(IInvocation castleInvocation, Type declaringType)
        {
            _castleInvocation = castleInvocation;
            _targetMethodProvider = new Lazy<MethodInfo>(() => _castleInvocation.GetConcreteMethodInvocationTarget());
         
            DeclaringType = declaringType;
            InvocationType = Method.GetInvocationType();
        }
    }
}