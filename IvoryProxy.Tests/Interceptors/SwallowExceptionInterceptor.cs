using System;
using IvoryProxy.Core.Interceptors;

namespace IvoryProxy.Tests.Interceptors
{
    public class SwallowExceptionInterceptor : ExceptionInterceptor
    {
        /// <inheritdoc />
        protected override void OnException(Exception exception)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override bool CanSwallowException(Exception exception)
        {
            return true;
        }
    }
}