using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Components;
using IvorySharp.Extensions.ClassAspectSelection;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Selection;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Weaving;
using IvorySharp.Tests.Assets;
using Xunit;

namespace IvorySharp.Tests.IntegrationTests
{ 
    /// <summary>
    /// Набор тестов для расшинерия ClassAspectsSelection.
    /// </summary>
    public class ClassAspectsSelectionTests
    {
        private readonly Weaved<ICommandParser, ConcreteCommandParser> _parserProvider;

        public ClassAspectsSelectionTests()
        {
            _parserProvider = new Weaved<ICommandParser, ConcreteCommandParser>(
                new TargetAspectSelectionComponents(
                    new DefaultComponentsStore(new NullDependencyProvider())), 
                a => a.UseClassAspectSelection());
        }
        
        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        public void Test_ExceptionChain_Correct(FrameworkType frameworkType)
        {
            // Arrange
            var parser = _parserProvider.Get(frameworkType);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => parser.Parse(""));

            Assert.IsType<ApplicationException>(exception.InnerException);
            Assert.IsType<ArgumentException>(exception.InnerException.InnerException);
            Assert.IsType<Exception>(exception.InnerException.InnerException.InnerException);
        }
        
        
        #region Services

        public interface ICommandParser
        {
            void Parse(string command);
        }
        
        [RethrowInvalidOperationExceptionAspect(Order = 2)] // Выполнится третим
        public abstract class BaseCommandParser : ICommandParser
        {
            public abstract void Parse(string command);
        }

        [RethrowApplicationExceptionAspect(Order = 1)] // Выполнится вторым
        public class ConcreteCommandParser : BaseCommandParser
        {
            [RethrowArgumentExceptionAspect(Order = 0)] // Выполнится первым
            public override void Parse(string command)
            {
                throw new Exception();
            }
        }
    
        #endregion

        #region Aspects

        internal class RethrowArgumentExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.RethrowException(new ArgumentException(string.Empty, pipeline.CurrentException));
            }
        }
        
        internal class RethrowApplicationExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.RethrowException(new ApplicationException(string.Empty, pipeline.CurrentException));
            }
        }
        
        internal class RethrowInvalidOperationExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.RethrowException(new InvalidOperationException(string.Empty, pipeline.CurrentException));
            }
        }

        #endregion

        #region Settings

        internal class TargetAspectSelectionComponents : IComponentsStore
        {
            public IComponentProvider<IDependencyProvider> DependencyProvider { get; }
            public IComponentProvider<IAspectSelector> AspectSelector { get; }
            public IComponentProvider<IAspectWeavePredicate> AspectWeavePredicate { get; }
            public IComponentProvider<IAspectDeclarationCollector> AspectDeclarationCollector { get; }
            public IComponentProvider<IInvocationPipelineFactory> PipelineFactory { get; }
            public IComponentProvider<IAspectFactory<MethodBoundaryAspect>> BoundaryAspectFactory { get; }
            public IComponentProvider<IAspectFactory<MethodInterceptionAspect>> InterceptionAspectFactory { get; }
            public IComponentProvider<IAspectDependencyInjector> AspectDependencyInjector { get; }
            public IComponentProvider<IAspectOrderStrategy> AspectOrderStrategy { get; }

            public TargetAspectSelectionComponents(IComponentsStore componentsStore)
            {
                DependencyProvider = componentsStore.DependencyProvider;
                AspectSelector = componentsStore.AspectSelector;
                AspectWeavePredicate = componentsStore.AspectWeavePredicate;
                AspectDeclarationCollector = componentsStore.AspectDeclarationCollector;
                PipelineFactory = componentsStore.PipelineFactory;
                BoundaryAspectFactory = componentsStore.BoundaryAspectFactory;
                InterceptionAspectFactory = componentsStore.InterceptionAspectFactory;
                AspectDependencyInjector = componentsStore.AspectDependencyInjector;
                AspectOrderStrategy = componentsStore.AspectOrderStrategy;
            
                componentsStore.AspectWeavePredicate
                    .Replace(new TargetTypeWeavePredicate(AspectSelector));
            
                componentsStore.AspectDeclarationCollector
                    .Replace(new TargetTypeAspectDeclarationCollector(AspectSelector));
            }
        }


        #endregion
    }
}