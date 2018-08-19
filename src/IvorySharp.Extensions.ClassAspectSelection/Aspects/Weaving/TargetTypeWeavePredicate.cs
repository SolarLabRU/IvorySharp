using System;
using System.Linq;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Extensions.ClassAspectSelection.Extensions;

namespace IvorySharp.Extensions.ClassAspectSelection.Aspects.Weaving
{
    /// <summary>
    /// Определяет возможность применения аспектов на основе целевого типа.
    /// </summary>
    internal sealed class TargetTypeWeavePredicate : BaseWeavePredicate
    {
        /// <summary>
        /// Инициализирует экземпляр <see cref="TargetTypeWeavePredicate"/>.
        /// </summary>
        public TargetTypeWeavePredicate(IComponentProvider<IAspectSelector> selectorProvider) 
            : base(selectorProvider)
        {
        }     
        
        /// <inheritdoc />
        public override bool IsWeaveable(Type declaringType, Type targetType)
        {
            // В любом случае объявленный тип должен быть интерфейсом
            // это ограничение прокси-генератора, да и C#-а в целом
            if (!declaringType.IsInterface)
                return false;

            if (!targetType.IsClass || targetType.IsAbstract)
                return false;
            
            if (IsWeavingSuppressed(targetType))
                return false;

            var aspectSelector = AspectSelectorProvider.Get();
            
            // На интерфейсе есть аспект
            if (aspectSelector.HasAnyAspect(targetType, includeAbstract: false))
                return true;

            // На методах класса есть аспекты
            if (targetType.GetMethods().Any(
                m => !IsWeavingSuppressed(m) && 
                     aspectSelector.HasAnyAspect(m, includeAbstract: false)))
                return true;

            var baseTypes = targetType.GetInterceptableBaseTypes().Where(t => !IsWeavingSuppressed(t)).ToArray();

            // На базовых типах есть аспекты
            if (baseTypes.Any(t => aspectSelector.HasAnyAspect(t, includeAbstract: false)))
                return true;

            // На методах базового типа есть аспекты
            return baseTypes.SelectMany(i => i.GetMethods())
                .Any(m => !IsWeavingSuppressed(m) && 
                          aspectSelector.HasAnyAspect(m, includeAbstract: false));
        }

        /// <inheritdoc />
        public override bool IsWeaveable(IInvocation invocation)
        {
            if (!invocation.DeclaringType.IsInterface ||
                !invocation.TargetType.IsClass ||
                invocation.TargetType.IsAbstract)
            {
                return false;
            }

            if (IsWeavingSuppressed(invocation.TargetMethod))
                return false;

            var aspectSelector = AspectSelectorProvider.Get();
            
            return aspectSelector.HasAnyAspect(invocation.TargetMethod, includeAbstract: false) || 
                   IsWeaveable(invocation.DeclaringType, invocation.TargetType);
        }
    }
}