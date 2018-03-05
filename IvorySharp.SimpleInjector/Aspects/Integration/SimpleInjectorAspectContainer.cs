using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Integration;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Core;
using SimpleInjector;
using IServiceProvider = IvorySharp.Aspects.Dependency.IServiceProvider;

namespace IvorySharp.SimpleInjector.Aspects.Integration
{
    /// <summary>
    /// Экземпляр контейнера аспектов для SimpleInjector-а.
    /// </summary>
    public class SimpleInjectorAspectContainer : AspectsContainer
    {       
        private readonly Container _container;
        private readonly IServiceProvider _serviceProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="SimpleInjectorAspectContainer"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public SimpleInjectorAspectContainer(Container container)
        {
            _container = container;        
            _serviceProvider = new SimpleInjectorServiceProvider(container);
        }

        /// <inheritdoc />
        public override void BindAspects(IAspectsWeavingSettings settings)
        {  
            var weaver = new AspectWeaver(settings);
            Func<object, Type, object> proxier = (o, type) => weaver.Weave(o, type);
            
            _container.ExpressionBuilt += (sender, args) =>
            { 
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

        /// <inheritdoc />
        public override IServiceProvider GetServiceProvider()
        {
            return _serviceProvider;
        }
    }
}