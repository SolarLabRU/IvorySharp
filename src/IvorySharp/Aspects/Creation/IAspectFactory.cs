﻿using IvorySharp.Core;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Компонент, выполняющий создание аспектов.
    /// </summary>
    public interface IAspectFactory
    {
        /// <summary>
        /// Создает аспекты типа <see cref="MethodBoundaryAspect"/> из модели вызова <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Массив инициализированных аспектов.</returns>
        MethodBoundaryAspect[] CreateBoundaryAspects(InvocationContext context);

        /// <summary>
        /// Создает аспект типа <see cref="MethodInterceptionAspect"/> из модели вызова <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст вызова.</param>
        /// <returns>Инициализированный аспект.</returns>
        MethodInterceptionAspect CreateInterceptionAspect(InvocationContext context);
    }
}