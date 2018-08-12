using System;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Вспомогательный класс для передачи параметров между сгенерированным прокси и методом перехвата вызова.
    /// </summary>
    internal class PackedArguments
    {
        /// <summary>
        /// Количество параметров.
        /// </summary>
        internal static readonly int Count = Enum.GetNames(typeof(PackedArgPosition)).Length;

        private readonly object[] _arguments;

        /// <summary>
        /// Инициализирует экземпляр <see cref="PackedArguments"/>.
        /// </summary>
        /// <param name="arguments">Упакованный массив параметров.</param>
        public PackedArguments(object[] arguments)
        {
            _arguments = arguments;
        }

        /// <summary>
        /// Экземпляр прокси.
        /// </summary>
        public IvoryProxy Proxy => Get<IvoryProxy>(PackedArgPosition.Proxy);

        /// <summary>
        /// Токен метода.
        /// </summary>
        public MethodToken MethodToken => new MethodToken(
            Get<int>(PackedArgPosition.MethodTokenKey),
            Get<Type>(PackedArgPosition.DeclaringType)
        );

        /// <summary>
        /// Параметры метода.
        /// </summary>
        public object[] MethodArguments => Get<object[]>(PackedArgPosition.MethodArguments);

        /// <summary>
        /// Обобщенные параметры.
        /// </summary>
        public Type[] GenericTypes => Get<Type[]>(PackedArgPosition.GenericArgs);

        /// <summary>
        /// Возвращаемое значение.
        /// </summary>
        public object ReturnValue
        {
            // ReSharper disable once UnusedMember.Global
            get => Get<object>(PackedArgPosition.ReturnValue);
            set => Set(PackedArgPosition.ReturnValue, value);
        }

        /// <summary>
        /// Получает параметр по индексу.
        /// </summary>
        /// <param name="position">Позиция параметра.</param>
        /// <typeparam name="T">Тип параметр.</typeparam>
        /// <returns>Параметр.</returns>
        private T Get<T>(PackedArgPosition position)
        {
            return (T) _arguments[(int) position];
        }

        /// <summary>
        /// Устанавливает значение параметра.
        /// </summary>
        /// <param name="position">Позиция параметра.</param>
        /// <param name="value">Значение параметра.</param>
        /// <typeparam name="T">Тип параметра.</typeparam>
        private void Set<T>(PackedArgPosition position, T value)
        {
            _arguments[(int) position] = value;
        }
    }
}