using System.ComponentModel;
using JetBrains.Annotations;

namespace IvorySharp.Linq
{
    /// <summary>
    /// Делегат вызова метода.
    /// </summary>
    /// <param name="target">Экземпляр класса на котором необходимо вызвать метод.</param>
    /// <param name="args">Параметры вызова.</param>
    [CanBeNull, EditorBrowsable(EditorBrowsableState.Never)]
    public delegate object MethodLambda([NotNull] object target, [NotNull] object[] args);
}