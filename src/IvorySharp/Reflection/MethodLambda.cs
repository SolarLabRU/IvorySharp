using System.ComponentModel;

namespace IvorySharp.Reflection
{
    /// <summary>
    /// Делегат вызова метода.
    /// </summary>
    /// <param name="target">Экземпляр класса на котором необходимо вызвать метод.</param>
    /// <param name="args">Параметры вызова.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate object MethodLambda(object target, object[] args);
}