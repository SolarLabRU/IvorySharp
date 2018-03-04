using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Integration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Core;
using SimpleInjector;

namespace IvorySharp.SimpleInjector.Aspects.Integration
{
    /// <summary>
    /// Экземпляр контейнера аспектов для SimpleInjector-а.
    /// </summary>
    public class SimpleInjectorAspectContainer : AspectsContainer
    {       
        private static Type[] _notWeavableTypes = new[]
        {
            typeof(IMethodAspect),
            typeof(IMethodBoundaryAspect),
            typeof(IInterceptor)
        };
        
        private readonly Container _container;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="SimpleInjectorAspectContainer"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public SimpleInjectorAspectContainer(Container container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public override void BindAspects(IWeavingAspectsConfiguration configuration)
        {  
            var weaver = new AspectWeaver(configuration);
            Func<object, Type, object> proxier = (o, type) => weaver.Weave(o, type);
            
            _container.ExpressionBuilt += (sender, args) =>
            {
                if (!args.RegisteredServiceType.IsInterface)
                    return;
                
                if (_notWeavableTypes.Contains(args.RegisteredServiceType))
                    return;

                if (configuration.ExplicitWeaingAttributeType != null &&
                    args.RegisteredServiceType.GetCustomAttribute(configuration.ExplicitWeaingAttributeType) == null)
                {
                    return;
                }

                args.Expression = Expression.Convert(
                    Expression.Invoke(
                        Expression.Constant(proxier),
                        args.Expression,
                        Expression.Constant(args.RegisteredServiceType, typeof(Type))
                    ), 
                    args.RegisteredServiceType
                );
            };
        }
    }
}