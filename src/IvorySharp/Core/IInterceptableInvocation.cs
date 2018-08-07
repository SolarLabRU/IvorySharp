namespace IvorySharp.Core
{
    /// <summary>
    /// Интерфейс выполнения пайплайна с поддержкой перехвата вызова исходного метода.
    /// </summary>
    public interface IInterceptableInvocation : IInvocation
    {
        /// <summary>
        /// Устанавливает возвращаемое значение вызова.
        /// </summary>
        /// <param name="returnValue">Возвращаемое значение.</param>
        void SetReturnValue(object returnValue);
    }
}