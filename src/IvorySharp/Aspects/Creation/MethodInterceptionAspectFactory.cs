using IvorySharp.Aspects.Dependency;
using IvorySharp.Components;
using IvorySharp.Core;
using IvorySharp.Exceptions;

namespace IvorySharp.Aspects.Creation
{
    /// <summary>
    /// Фабрика аспектов типа <see cref="MethodInterceptionAspectFactory"/>.
    /// </summary>
    internal sealed class MethodInterceptionAspectFactory : IAspectFactory<MethodInterceptionAspect>
    {
        private readonly IComponentProvider<IAspectDependencyInjector> _dependencyInjectorProvider;
        private readonly IComponentProvider<IAspectPreInitializer<MethodInterceptionAspect>> _preInitializerProvider;

        /// <summary>
        /// Инициализирует фабрику аспектов.
        /// </summary>
        public MethodInterceptionAspectFactory(
            IComponentProvider<IAspectDependencyInjector> dependencyInjectorProvider,
            IComponentProvider<IAspectPreInitializer<MethodInterceptionAspect>> preInitializerProvider)
        {
            _dependencyInjectorProvider = dependencyInjectorProvider;
            _preInitializerProvider = preInitializerProvider;
        }

        /// <inheritdoc />
        public MethodInterceptionAspect[] CreateAspects(IInvocationContext context)
        {
            var preInitializer = _preInitializerProvider.Get();
            var dependencyInjector = _dependencyInjectorProvider.Get();
            var aspects = preInitializer.PrepareAspects(context);
            
            if (aspects.Length > 1)
                throw new IvorySharpException(
                    $"Допустимо наличие только одного аспекта типа '{typeof(MethodInterceptionAspect)}'. " +
                    $"На методе '{context.Method.Name}' типа '{context.DeclaringType.FullName}' задано несколько.");

            foreach (var aspect in aspects)
            {
                dependencyInjector.InjectPropertyDependencies(aspect);
                aspect.Initialize();
            }

            return aspects;
        }
    }
}