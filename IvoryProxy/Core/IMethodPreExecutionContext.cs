using System.Reflection;

namespace IvoryProxy.Core
{
    /// <summary>
    /// Контекст метода до выполнения.
    /// </summary>
    public interface IMethodPreExecutionContext
    {
        /// <summary>
        /// Экземпляр целевого объекта, метод которого был вызван.
        /// </summary>
        object InvocationTarget { get; }
        
        /// <summary>
        /// Массив параметров, с которыми был вызван метод.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Метод, вызов которого был запрошен.
        /// </summary>
        MethodInfo TargetMethod { get; }
    }
}