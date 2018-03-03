using System;
using System.Runtime.ExceptionServices;

namespace IvorySharp.Extensions
{
    /// <summary>
    /// Набор расширений для исключений.
    /// </summary>
    internal static class ExceptionExtensions
    {
        public static Exception Unwrap(this Exception exception)
        {
            return exception.InnerException ?? exception;
        }

        public static Exception UnwrapIf(this Exception exception, bool condition)
        {
            return condition ? exception.Unwrap() : exception;
        }

        public static void Rethrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}