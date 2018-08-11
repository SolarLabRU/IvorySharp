using System;
using System.Linq.Expressions;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Integration;
using IvorySharp.Aspects.Weaving;
using SimpleInjector;

namespace IvorySharp.SimpleInjector.Aspects.Integration
{
    /// <summary>
    /// Экземпляр контейнера аспектов для SimpleInjector-а.
    /// </summary>
    public class SimpleInjectorAspectContainer : AspectsContainer
    {       
        private readonly Container _container;
        private readonly IDependencyProvider _dependencyProvider;
        
        /// <summary>
        /// Инициализирует экземпляр <see cref="SimpleInjectorAspectContainer"/>.
        /// </summary>
        /// <param name="container">Контейнер зависимостей.</param>
        public SimpleInjectorAspectContainer(Container container)
        {
            _container = container;        
            _dependencyProvider = new SimpleInjectorDependencyProvider(container);
        }

        /// <inheritdoc />
        public override void BindAspects(IComponentsStore components)
        {  
            var weaver = new AspectWeaver(components.AspectWeavePredicate, components.PipelineFactory, components.AspectFactory);

            object Proxier(object o, Type declaredType, Type targetType) => weaver.Weave(o, declaredType, targetType);

            _container.ExpressionBuilt += (sender, args) =>
            {
                var producer = _container.GetRegistration(args.RegisteredServiceType);
                var implementationType = producer?.Registration.ImplementationType;

                args.Expression = Expression.Convert(
                    Expression.Invoke(
                        Expression.Constant((Func<object, Type, Type, object>) Proxier),
                        args.Expression,
                        Expression.Constant(args.RegisteredServiceType, typeof(Type)),
                        Expression.Constant(implementationType, typeof(Type))
                    ), 
                    args.RegisteredServiceType
                );
            };
        }

        /// <inheritdoc />
        public override IDependencyProvider GetDependencyProvider()
        {
            return _dependencyProvider;
        }
    }
}