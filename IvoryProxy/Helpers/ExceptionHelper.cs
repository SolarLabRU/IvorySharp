using System;
using System.Reflection;

namespace IvoryProxy.Helpers
{
    internal static class ExceptionHelper
    {
        public static Exception UnwrapTargetInvocationException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            
            if (exception is TargetInvocationException e && e.InnerException != null)
                return e.InnerException;
            return exception;
        }
    }
}