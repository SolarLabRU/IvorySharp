namespace IvorySharp.Aspects.Pipeline
{   
    /// <summary>
    /// Состояние пайлайна вызова.
    /// </summary>
    internal enum InvocationPipelineState
    {
        /// <summary>
        /// Нормальное выполнение пайплайна.
        /// </summary>
        Continue = 0,
        
        /// <summary>
        /// Пайплайн прерван с возвратом результата.
        /// </summary>
        Return = 1,
        
        /// <summary>
        /// Пайплайн прерван с исключением.
        /// </summary>
        Exception = 2
    }
}