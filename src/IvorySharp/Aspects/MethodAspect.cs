using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Components.Weaving;
using IvorySharp.Extensions;

namespace IvorySharp.Aspects
{
    /// <summary>
    /// Базовый класс для аспектов, применяемых на уровне метода.
    /// </summary>
    public abstract class MethodAspect : AspectAttribute
    {
        /// <summary>
        /// Признак наличия зависимостей.
        /// </summary>
        internal bool HasDependencies { get; set; }

        /// <summary>
        /// Описание аспекта.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Выполняет инициализацию аспекта.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Возвращает аспекты уровня метода.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="method">Ссылка на метод.</param>
        /// <returns>Перечень аспектов.</returns>
        internal static IEnumerable<TAspect> GetMethodAspects<TAspect>(MethodInfo method) 
            where TAspect : MethodAspect
        {
            return method.GetCustomAttributes<TAspect>(inherit: false);
        }

        /// <summary>
        /// Возвращает аспекты уровня метода типов, входящих в иерархию наследования от типа <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="type">Ссылка на тип.</param>
        /// <returns>Перечень аспектов.</returns>
        internal static IEnumerable<TAspect> GetTypeHierarchyMethodAspects<TAspect>(Type type)
            where TAspect : MethodAspect
        {
            return new[] { type }.Concat(type.GetInterfaces()) 
                .Where(t => !AspectWeaver.NotWeavableTypes.Contains(t) && 
                            t.GetCustomAttributes<SuppressAspectsWeavingAttribute>().IsEmpty())
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes<SuppressAspectsWeavingAttribute>().IsEmpty())
                .SelectMany(m => m.GetCustomAttributes<TAspect>(inherit: false));
        }

        /// <summary>
        /// Возвращает аспекты уровня типа всей иерархии от типа <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TAspect">Тип аспекта.</typeparam>
        /// <param name="type">Ссылка на тип.</param>
        /// <returns>Перечень аспектов.</returns>
        internal static IEnumerable<TAspect> GetTypeHierarchyAspects<TAspect>(Type type)
            where TAspect : MethodAspect
        {
            return type.GetCustomAttributes<TAspect>(inherit: false)
                .Concat(type
                        .GetInterfaces()
                        .Where(t => !AspectWeaver.NotWeavableTypes.Contains(t) && 
                                    t.GetCustomAttributes<SuppressAspectsWeavingAttribute>().IsEmpty())
                        .SelectMany(t => t.GetCustomAttributes<TAspect>(inherit: false)));
        }
    }
}