using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Configuration;
using IvorySharp.Core;
using IvorySharp.Extensions;
using IvorySharp.Proxying;

namespace IvorySharp.Aspects.Weaving
{
    /// <summary>
    /// Компонент для выполнения связывания исходного объекта с аспектами.
    /// </summary>
    public class AspectWeaver
    {
        /// <summary>
        /// Массив типов для которых нельзя применять обвязку.
        /// </summary>
        public static readonly Type[] NotWeavableTypes = {
            typeof(IInterceptor),
            typeof(IInvocation),
            typeof(IServiceProvider),
            typeof(IInvocationPipeline)
        };

        /// <summary>
        /// Массив методов, которые нельзя перехватывать.
        /// </summary>
        public static readonly MethodInfo[] NotInterceptableMethods =
        {
            typeof(object).GetMethod(nameof(GetType)),
            typeof(object).GetMethod(nameof(ReferenceEquals))
        };
        
        private readonly IAspectsWeavingSettings _configuration;

        /// <summary>
        /// Инициализирует экземпляр <see cref="AspectWeaver"/>.
        /// </summary>
        /// <param name="configuration">Конфигурация аспектов.</param>
        public AspectWeaver(IAspectsWeavingSettings configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Выполняет связывание исходного объекта с заданными для него аспектами.
        /// </summary>
        /// <param name="target">Экземпляр исходного объекта.</param>
        /// <param name="targetDeclaredType">Объявленный тип исходного объекта.</param>
        /// <returns>Экземпляр связанного с аспектами исходного объекта типа <paramref name="targetDeclaredType"/>.</returns>
        public object Weave(object target, Type targetDeclaredType)
        {
            if (!IsWeavable(targetDeclaredType, _configuration))
                return target;
            
            var interceptor = new AspectWeaveInterceptor(_configuration);
            return InterceptProxyGenerator.Default.CreateInterceptProxy(target, targetDeclaredType, interceptor);
        }

        /// <summary>
        /// Возвращает признак возможности применения обвязки для указанного типа.
        /// </summary>
        /// <param name="invocationContext">Контекст вызова.</param>
        /// <param name="settings">Настройки.</param>
        /// <returns>Признак возможности применения обвязки для указанного типа.</returns>
        public static bool IsWeavable(InvocationContext invocationContext, IAspectsWeavingSettings settings)
        {
            if (!invocationContext.InstanceDeclaringType.IsInterface)
                return false;

            if (NotWeavableTypes.Contains(invocationContext.InstanceDeclaringType))
                return false;
            
            // Если включена настройка явного указания атрибута для обвязки
            if (settings.ExplicitWeavingAttributeType != null)
            {
                var explicitMarkers = invocationContext.InstanceDeclaringType.GetCustomAttributes(
                    settings.ExplicitWeavingAttributeType, inherit: false);
                
                if (explicitMarkers.IsEmpty())
                    return false;
            }
            
            var suppressWeavingAttribute = invocationContext.InstanceDeclaringType.GetCustomAttributes<SuppressAspectsWeavingAttribute>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return false;

            suppressWeavingAttribute = invocationContext.Method.GetCustomAttributes<SuppressAspectsWeavingAttribute>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return false;

            var hasAttribute =
                MethodAspect.GetMethodAspects<MethodAspect>(invocationContext.Method).IsNotEmpty() ||
                MethodAspect.GetTypeHierarchyAspects<MethodAspect>(invocationContext.InstanceDeclaringType).IsNotEmpty() ||
                MethodAspect.GetTypeHierarchyMethodAspects<MethodAspect>(invocationContext.InstanceDeclaringType).IsNotEmpty();

            return hasAttribute;
        }
        
        /// <summary>
        /// Возвращает признак возможности применения обвязки для указанного типа.
        /// </summary>
        /// <param name="type">Тип для применения аспектов.</param>
        /// <param name="settings">Настройки.</param>
        /// <returns>Признак возможности применения обвязки для указанного типа.</returns>
        public static bool IsWeavable(Type type, IAspectsWeavingSettings settings)
        {
            if (!type.IsInterface)
                return false;

            if (NotWeavableTypes.Contains(type))
                return false;
            
            // Если включена настройка явного указания атрибута для обвязки
            if (settings.ExplicitWeavingAttributeType != null)
            {
                var explicitMarkers = type.GetCustomAttributes(settings.ExplicitWeavingAttributeType, inherit: false);
                if (explicitMarkers.IsEmpty())
                    return false;
            }

            // Если тип помечен атрибутом, который запрещает применение аспектов
            var suppressWeavingAttribute = type.GetCustomAttributes<SuppressAspectsWeavingAttribute>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return false;

            var aspectAttribute = MethodAspect.GetTypeHierarchyAspects<MethodAspect>(type);
            if (aspectAttribute.IsNotEmpty())
                return true;

            var methodAspectAttributes = MethodAspect.GetTypeHierarchyMethodAspects<MethodAspect>(type);
            if (methodAspectAttributes.IsNotEmpty())
                return true;

            return false;
        }
    }
}