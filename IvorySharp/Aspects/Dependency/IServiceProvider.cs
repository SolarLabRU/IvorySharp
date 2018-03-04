﻿using System;

namespace IvorySharp.Aspects.Dependency
{
    /// <summary>
    /// Провайдер сервисов (сервис локатор).
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// Получает экземпляр сервиса по типу.
        /// </summary>
        /// <typeparam name="TService">Тип сервиса.</typeparam>
        /// <returns>Экземпляр сервиса.</returns>
        TService GetService<TService>() where TService : class;
        
        /// <summary>
        /// Получает экземпляр именованного сервиса.
        /// </summary>
        /// <param name="key">Ключ сервиса.</param>
        /// <typeparam name="TService">Тип сервиса.</typeparam>
        /// <returns>Экземпляр сервиса.</returns>
        TService GetNamedService<TService>(string key) where TService : class;    
        
        /// <summary>
        /// Получает экземпляр сервиса по типу.
        /// </summary>
        /// <param name="serviceType">Тип сервиса.</param>
        /// <returns>Экземпляр сервиса.</returns>
        object GetService(Type serviceType);
        
        /// <summary>
        /// Получает экземпляр сервиса по типу.
        /// </summary>
        /// <param name="serviceType">Тип сервиса.</param>
        /// <param name="key">Ключ сервиса.</param>
        /// <returns>Экземпляр сервиса.</returns>
        object GetNamedService(Type serviceType, string key);
    }
}