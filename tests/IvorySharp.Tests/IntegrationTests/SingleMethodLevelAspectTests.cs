using System;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Configuration;
using IvorySharp.Aspects.Dependency;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Components;
using IvorySharp.Tests.Assets;
using Xunit;

namespace IvorySharp.Tests.IntegrationTests
{
    /// <summary>
    /// Набор интеграционных тестов на сценарий, когда на методе закреплен один аспект.
    /// </summary>
    public class SingleMethodLevelAspectTests
    {
        private readonly Weaved<IService, Service> _serviceProvider;

        public SingleMethodLevelAspectTests()
        {
            IComponentsStore componentsStore = new DefaultComponentsStore(NullDependencyProvider.Instance);
            _serviceProvider = new Weaved<IService, Service>(componentsStore);
        }


        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void OnSuccess_ChangeReturnResult_By_ReturnValueProp(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);
            
            // Act
            var result = service.ShouldReturnArgumentPlusOne(10);
            
            // Assert
            Assert.Equal(11, result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void BypassAspect_Should_Not_SwallowException_If_It_WasThrown_By_Method(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ShouldThrowArgumentException());        
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void SwallowException_In_OnExceptionBlock_ShouldReturnDefault(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act
            var result = service.ShouldNotThrowArgumentException();
            
            // Assert
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void OnEntry_ReturnValue_ShouldNotTriggerMethod_Invocation(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act
            var result = service.ShouldReturnNewObjectInsteadOfNull();
            
            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(FrameworkType.Native)]
        [InlineData(FrameworkType.SimpleInjector)]
        [InlineData(FrameworkType.CastleWindsor)]
        [InlineData(FrameworkType.MicrosoftDependencyInjection)]
        public void ThrowException_In_OnSuccessBlock_ShouldFaultInvocation(FrameworkType frameworkType)
        {
            // Arrange
            var service = _serviceProvider.Get(frameworkType);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.ShouldThrowArgumentNullException(10));
        }
        
        #region Aspects

        private class SwallowExceptionAspect : MethodBoundaryAspect
        {
            public override void OnException(IInvocationPipeline pipeline)
            {
                pipeline.Return();
            }
        }
        
        private class BypassAspect : MethodBoundaryAspect
        {
            
        }
        
        private class IncrementValueOnSuccessAspects : MethodBoundaryAspect
        {
            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                var value = (int) pipeline.CurrentReturnValue;
                pipeline.Return(value + 1);
            }
        }
        
        private class ReturnObjectOnEntryAspect : MethodBoundaryAspect
        {
            public override void OnEntry(IInvocationPipeline pipeline)
            {
                pipeline.Return(new object());
            }
        }

        private class ThrowArgumentNullExceptionOnSuccess : MethodBoundaryAspect
        {
            public override void OnSuccess(IInvocationPipeline pipeline)
            {
                throw new ArgumentNullException();
            }
        }

        #endregion

        #region Services

        public interface IService
        {
            [IncrementValueOnSuccessAspects]
            int ShouldReturnArgumentPlusOne(int argument);

            [BypassAspect]
            void ShouldThrowArgumentException();

            [SwallowExceptionAspect]
            int ShouldNotThrowArgumentException();

            [ReturnObjectOnEntryAspect]
            object ShouldReturnNewObjectInsteadOfNull();

            [ThrowArgumentNullExceptionOnSuccess]
            int ShouldThrowArgumentNullException(int argument);
        }
        
        public class Service : IService
        {
            public int ShouldReturnArgumentPlusOne(int argument)
            {
                return argument;
            }

            public void ShouldThrowArgumentException()
            {
                throw new ArgumentException();
            }

            public int ShouldNotThrowArgumentException()
            {
                throw new ArgumentException();
            }

            public object ShouldReturnNewObjectInsteadOfNull()
            {
                return null;
            }

            public int ShouldThrowArgumentNullException(int argument)
            {
                return argument;
            }
        }

        #endregion
    }
}