using System.ComponentModel;
using System.Threading.Tasks;

namespace IvorySharp.Core
{
    /// <summary>
    /// Тип метода.
    /// </summary>
    public enum InvocationType
    {
        /// <summary>
        /// Синхронный метод.
        /// </summary>
        Synchronous = 0,
        
        /// <summary>
        /// Асинхронная функция (return: <see cref="Task{TResult}"/>)
        /// </summary>
        AsyncFunction = 1,
        
        /// <summary>
        /// Асинхронное действие (return <see cref="Task"/>)
        /// </summary>
        AsyncAction = 2
    }
}