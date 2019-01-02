using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace IvorySharp.Core
{
    /// <summary>
    /// Represents a collection of arguments passed to the method.
    /// </summary>
    [PublicAPI]
    public sealed class InvocationArguments : IReadOnlyCollection<object>
    {
        /// <summary>
        /// An empty instance of <see cref="InvocationArguments"/>.
        /// </summary>
        public static readonly InvocationArguments Empty = new InvocationArguments(Array.Empty<object>());
        
        private readonly object[] _args;
        
        /// <summary>
        /// Creates instance of <see cref="InvocationArguments"/>.
        /// </summary>
        /// <param name="args">Array of untyped method arguments.</param>
        private InvocationArguments(object[] args)
        {
            _args = args ?? Array.Empty<object>();
        }

        /// <summary>
        /// Arguments count.
        /// </summary>
        public int Count => _args.Length;

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator()
        {
            return _args.AsEnumerable().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Implicitly converts <see cref="InvocationArguments"/> to array of <see cref="object"/>.
        /// </summary>
        /// <param name="arguments">Method arguments collection.</param>
        /// <returns>Untyped method arguments array.</returns>
        public static implicit operator object[]([NotNull] InvocationArguments arguments)
        {
            return arguments._args;
        }

        /// <summary>
        /// Implicitly converts an array of untyped method arguments to the <see cref="InvocationArguments"/>.
        /// </summary>
        /// <param name="args">Untyped method arguments array.</param>
        /// <returns>Method arguments collection.</returns>
        public static implicit operator InvocationArguments([NotNull] object[] args)
        {
            return new InvocationArguments(args);
        }
    }
}