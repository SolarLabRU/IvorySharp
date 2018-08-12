using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IvorySharp.Core
{
    /// <summary>
    /// Параметры вызова метода.
    /// </summary>
    public sealed class InvocationArguments : IReadOnlyCollection<object>
    {
        /// <summary>
        /// Инициализированный пустой экземпляр <see cref="InvocationArguments"/>.
        /// </summary>
        public static readonly InvocationArguments Empty = new InvocationArguments(Array.Empty<object>());
        
        private readonly object[] _args;
        
        /// <summary>
        /// Инициализирует экзмпляр <see cref="InvocationArguments"/>.
        /// </summary>
        /// <param name="args">Массив нетипизированных параметров.</param>
        private InvocationArguments(object[] args)
        {
            _args = args ?? Array.Empty<object>();
        }

        /// <summary>
        /// Количество параметров.
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
        /// Оператор приведения типа.
        /// </summary>
        /// <param name="arguments">Параметры вызова.</param>
        /// <returns>Массив нетипизированных параметров вызова.</returns>
        public static implicit operator object[](InvocationArguments arguments)
        {
            return arguments._args;
        }

        /// <summary>
        /// Оператор приведения типа.
        /// </summary>
        /// <param name="args">Массив нетипизированных параметров вызова.</param>
        /// <returns>Параметры вызова метода.</returns>
        public static implicit operator InvocationArguments(object[] args)
        {
            return new InvocationArguments(args);
        }
    }
}