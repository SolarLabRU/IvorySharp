using System;
using System.Reflection;
using IvorySharp.Caching;
using IvorySharp.Extensions;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Базовая модель вызываемого метода.
    /// </summary>
    public abstract class AbstractInvocation : IInvocation
    {
        /// <inheritdoc />
        public InvocationArguments Arguments { get; }

        /// <inheritdoc />
        public MethodInfo Method { get; }

        /// <inheritdoc />
        public MethodInfo TargetMethod => _targetMethodProvider.Value;
        private readonly Lazy<MethodInfo> _targetMethodProvider;

        /// <inheritdoc />
        public Type DeclaringType { get; }

        /// <inheritdoc />
        public Type TargetType { get; }

        /// <inheritdoc />
        public object Proxy { get; }

        /// <inheritdoc />
        public object Target { get; }
        
        /// <inheritdoc />
        public InvocationType InvocationType { get; }

        /// <inheritdoc />
        public abstract object ReturnValue { get; set; }

        /// <summary>
        /// Инициализирует экземпляр <see cref="AbstractInvocation"/>.
        /// </summary>
        protected internal AbstractInvocation(
            InvocationArguments arguments,
            MethodInfo proxiedMethod,
            Type declaringType,
            Type targetType,
            object proxy,
            object target)
        {
            Arguments = arguments;
            Method = proxiedMethod;
            DeclaringType = declaringType;
            TargetType = targetType;
            Proxy = proxy;
            Target = target;
            InvocationType = Method.GetInvocationType();           
            _targetMethodProvider = new Lazy<MethodInfo>(GetTargetMethod);
        }

        
        /// <inheritdoc />
        public abstract object Proceed();

        /// <summary>
        /// Возвращает метод в типе <see cref="TargetType"/>, соответствующий перехватываемому
        /// методу <see cref="Method"/>.
        /// </summary>
        /// <returns>Метод.</returns>
        [CanBeNull] protected MethodInfo GetTargetMethod()
        {
            if (TargetType != null && Method != null)
                return MethodCache.Instance.GetMethodMap(TargetType, Method);
            
            return null;
        }
    }
}