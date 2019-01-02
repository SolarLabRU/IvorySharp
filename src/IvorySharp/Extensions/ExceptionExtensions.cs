using System;
using System.Runtime.ExceptionServices;

namespace IvorySharp.Extensions
{
    /// <summary>
    /// A set of extenstion methods for <see cref="Exception"/>.
    /// </summary>
    internal static class ExceptionExtensions
    {
        internal static Exception GetInner(this Exception exception)
        {
            return exception.InnerException ?? exception;
        }

        internal static Exception GetInnerIf(this Exception exception, bool condition)
        {
            return condition ? exception.GetInner() : exception;
        }

        internal static void Throw(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}