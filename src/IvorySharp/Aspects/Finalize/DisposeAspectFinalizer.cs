using System;

namespace IvorySharp.Aspects.Finalize
{
    /// <summary>
    /// Финализатор аспекта, вызывающий Dispose.
    /// </summary>
    internal sealed class DisposeAspectFinalizer : IAspectFinalizer
    {
        /// <inheritdoc />
        public void Finalize(MethodAspect methodAspect)
        {
            if (methodAspect is IDisposable ds)
                ds.Dispose();
        }
    }
}