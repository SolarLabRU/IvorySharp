using System;
using Castle.Windsor;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using IvorySharp.Tests.Assets;
using SimpleInjector;
using Xunit;

namespace IvorySharp.Tests.IntegrationTests
{
    public partial class ComplexAspectTests
    {
        private readonly Weaved<IIntCommandExecutor, IntCommandExecutor> _commandExecutorProvider;
        private readonly Weaved<ICommandExecutor<int>, IntCommandExecutor> _commandExecutorBaseProvider;

        public ComplexAspectTests()
        {
            var windsorContainer = new WindsorContainer();
            var simpleInjectorContainer = new Container();
            
            _commandExecutorProvider = new Weaved<IIntCommandExecutor, IntCommandExecutor>(
                new DefaultComponentsStore(NullDependencyProvider.Instance), simpleInjectorContainer, windsorContainer);
            
            _commandExecutorBaseProvider = new Weaved<ICommandExecutor<int>, IntCommandExecutor>(
                new DefaultComponentsStore(NullDependencyProvider.Instance), simpleInjectorContainer, windsorContainer);
        }
        
        #region Services

        /// <summary>
        /// Должен выполниться сначала аспект
        /// RethrowArgumentExceptionAspect, затем RethrowApplicationExceptionAspect
        /// </summary>
        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void Test_ExceptionThrowed_ConcreteService_Resolve(FrameworkType frameworkType)
        {
            // Arrange
            var executor = _commandExecutorProvider.Get(frameworkType);
            var action = new IdentityCommand<int>();
            
            // Act
            var exception = Assert.Throws<ApplicationException>(
                () => executor.ExecuteAction(action, throwOnException: true));
            
            // Assert
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.IsType<Exception>(exception.InnerException.InnerException);
        }

        /// <summary>
        /// Должен выполниться только аспект RethrowArgumentExceptionAspect
        /// </summary>
        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void Test_ExceptionThrowed_BaseService_Resolve(FrameworkType frameworkType)
        {
            // Arrange
            var executor = _commandExecutorBaseProvider.Get(frameworkType);
            var action = new IdentityCommand<int>();
            
            // Act
            var exception = Assert.Throws<ArgumentException>(
                () => executor.ExecuteAction(action, throwOnException: true));
            
            // Assert
            Assert.IsType<Exception>(exception.InnerException);
        }
        
        public class IdentityCommand<T>
        {
            public T Result { get; private set; }
            
            public void Execute(T input, bool throwException)
            {
                if (throwException) 
                    throw new Exception();

                Result = input;
            }
        }
        
        public interface ICommandExecutor<T>
        {   
            [RethrowArgumentExceptionAspect(Order = 0)]
            void ExecuteAction(IdentityCommand<T> action, bool throwOnException);
        }
        
        [RethrowApplicationExceptionAspect(Order = 1)]
        public interface IGenericCommandExecutor<T> : ICommandExecutor<T>
        {
            T GetValue();
        }
       
        public interface IIntCommandExecutor : IGenericCommandExecutor<int>
        {
            
        }
        
        public class IntCommandExecutor : IIntCommandExecutor
        {
            
            public void ExecuteAction(IdentityCommand<int> action, bool throwOnException)
            {
                action.Execute(GetValue(), throwOnException);
            }

            public int GetValue()
            {
                return 10;
            }
        }

        #endregion

        #region Aspects

        private class RethrowArgumentExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.ContinueFaulted(new ArgumentException(string.Empty, pipeline.CurrentException));
            }
        }
        
        private class RethrowApplicationExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.ContinueFaulted(new ApplicationException(string.Empty, pipeline.CurrentException));
            }
        }

        #endregion
    }
}