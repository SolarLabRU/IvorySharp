﻿namespace IvorySharp.Aspects
{
    /// <summary>
    /// Интерфейс аспекта, применяемого на метод.
    /// </summary>
    public interface IMethodAspect
    {
        /// <summary>
        /// Описание аспекта.
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// Выполняет инициализацию аспекта.
        /// </summary>
        void Initialize();
    }
}