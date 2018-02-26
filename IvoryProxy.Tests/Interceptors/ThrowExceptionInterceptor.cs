using System;
using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class ThrowExceptionInterceptor<TException> : IMethodInterceptor where TException : Exception, new()
    {
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            throw new TException();
        }
    }
}