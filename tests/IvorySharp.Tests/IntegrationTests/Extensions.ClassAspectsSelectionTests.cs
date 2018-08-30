using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Creation;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Finalize;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using IvorySharp.Caching;
using IvorySharp.Components;
using IvorySharp.Extensions.ClassAspectSelection;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Selection;
using IvorySharp.Extensions.ClassAspectSelection.Aspects.Weaving;
using IvorySharp.Tests.Assets;
using IvorySharp.Tests.Assets.Cache;
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
                    new DefaultComponentsStore(NullDependencyProvider.Instance)), 
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
                pipeline.Continue(new ArgumentException(string.Empty, pipeline.CurrentException));
            }
        }
        
        internal class RethrowApplicationExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.Continue(new ApplicationException(string.Empty, pipeline.CurrentException));
            }
        }
        
        internal class RethrowInvalidOperationExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.Continue(new InvalidOperationException(string.Empty, pipeline.CurrentException));
            }
        }

        #endregion

        #region Settings

        internal class TargetAspectSelectionComponents : IComponentsStore
        {
            public IComponentHolder<IDependencyProvider> DependencyHolder { get; }
            public IComponentHolder<IAspectSelector> AspectSelector { get; }
            public IComponentHolder<IAspectWeavePredicate> AspectWeavePredicate { get; }
            public IComponentHolder<IAspectDeclarationCollector> AspectDeclarationCollector { get; }
            public IComponentHolder<IInvocationPipelineFactory> PipelineFactory { get; }
            public IComponentHolder<IAspectFactory> AspectFactory { get; }
            public IComponentHolder<IAspectDependencyInjector> AspectDependencyInjector { get; }
            public IComponentHolder<IAspectOrderStrategy> AspectOrderStrategy { get; }
            public IComponentHolder<IInvocationWeaveDataProviderFactory> WeaveDataProviderFactory { get; }
            public IComponentHolder<IAspectFinalizer> AspectFinalizer { get; }

            public TargetAspectSelectionComponents(IComponentsStore componentsStore)
            {
                DependencyHolder = componentsStore.DependencyHolder;
                AspectSelector = componentsStore.AspectSelector;
                AspectWeavePredicate = componentsStore.AspectWeavePredicate;
                AspectDeclarationCollector = componentsStore.AspectDeclarationCollector;
                PipelineFactory = componentsStore.PipelineFactory;
                AspectFactory = componentsStore.AspectFactory;
                AspectDependencyInjector = componentsStore.AspectDependencyInjector;
                AspectOrderStrategy = componentsStore.AspectOrderStrategy;
                WeaveDataProviderFactory = componentsStore.WeaveDataProviderFactory;
                AspectFinalizer = componentsStore.AspectFinalizer;
            
                componentsStore.AspectWeavePredicate
                    .Replace(new TargetTypeWeavePredicate(AspectSelector));
            
                componentsStore.AspectDeclarationCollector
                    .Replace(new TargetTypeAspectDeclarationCollector(AspectSelector));
            }
        }


        #endregion
    }
}