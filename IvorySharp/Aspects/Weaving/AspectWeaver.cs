using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Pipeline;
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
        private static readonly ConcurrentDictionary<MethodInfo, bool> _weavableMethodsCached;

        static AspectWeaver()
        {
            _weavableMethodsCached = new ConcurrentDictionary<MethodInfo, bool>();
        }
        
        /// <summary>
        /// Массив типов для которых нельзя применять обвязку.
        /// </summary>
        public static readonly Type[] NotWeavableTypes = {
            typeof(IMethodAspect),
            typeof(IMethodBoundaryAspect),
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
        /// <param name="invocation">Вызов для применения обвязки.</param>
        /// <param name="settings">Настройки.</param>
        /// <returns>Признак возможности применения обвязки для указанного типа.</returns>
        public static bool IsWeavable(IInvocation invocation, IAspectsWeavingSettings settings)
        {
            bool CacheResultAndReturn(bool result)
            {
                _weavableMethodsCached.AddOrUpdate(invocation.Context.Method, result, (_, __) => result);
                return result;
            }
            
            if (_weavableMethodsCached.TryGetValue(invocation.Context.Method, out var isWeavable))
                return isWeavable;
            
            var ctx = invocation.Context;
            
            if (!ctx.InstanceDeclaringType.IsInterface)
                return CacheResultAndReturn(false);

            if (NotWeavableTypes.Contains(ctx.InstanceDeclaringType))
                return CacheResultAndReturn(false);
            
            // Если включена настройка явного указания атрибута для обвязки
            if (settings.ExplicitWeavingAttributeType != null)
            {
                var explicitMarkers = ctx.InstanceDeclaringType.GetCustomAttributes(
                    settings.ExplicitWeavingAttributeType, inherit: false);
                
                if (explicitMarkers.IsEmpty())
                    return CacheResultAndReturn(false);
            }
            
            var suppressWeavingAttribute = ctx.InstanceDeclaringType.GetCustomAttributes<SuppressWeaving>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return CacheResultAndReturn(false);

            suppressWeavingAttribute = ctx.Method.GetCustomAttributes<SuppressWeaving>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return CacheResultAndReturn(false);

            var hasAttribute = ctx.Method.GetCustomAttributes<MethodAspect>(inherit: false).IsNotEmpty() || 
                   ctx.InstanceDeclaringType.GetCustomAttributes<MethodAspect>(inherit: false).IsNotEmpty();

            return CacheResultAndReturn(hasAttribute);
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
            var suppressWeavingAttribute = type.GetCustomAttributes<SuppressWeaving>(inherit: false);
            if (suppressWeavingAttribute.IsNotEmpty())
                return false;
            
            var aspectAttribute = type.GetCustomAttributes<MethodAspect>(inherit: false);
            if (aspectAttribute.IsNotEmpty())
                return true;

            foreach (var method in type.GetMethods())
            {
                var methodAspectAttributes = method.GetCustomAttributes<MethodAspect>(inherit: false);
                if (methodAspectAttributes.IsNotEmpty())
                    return true;
            }

            return false;
        }
    }
}