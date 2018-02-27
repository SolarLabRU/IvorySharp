using System;
using IvoryProxy.Core;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class ThrowExceptionInterceptor<TException> : IvoryInterceptor where TException : Exception, new()
    {
        /// <inheritdoc />
        public override void Intercept(IInvocation invocation)
        {
            throw new TException();
        }

        public bool CanIntercept(IInvocation invocation)
        {
            return true;
        }
    }
}