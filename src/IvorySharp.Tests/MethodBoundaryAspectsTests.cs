using System;
using IvorySharp.Aspects;
using IvorySharp.Tests.Aspects;
using IvorySharp.Tests.Helpers;
using IvorySharp.Tests.Services;
using IvorySharp.Tests.WeavingSettings;
using Xunit;

namespace IvorySharp.Tests
{
    /// <summary>
    /// Набор тестов для аспектов типа <see cref="MethodBoundaryAspect"/>.
    /// </summary>
    /// TODO: Переписать на нормальные Юнит тесты + Интеграционные
    public class MethodBoundaryAspectsTests
    {
        private readonly WeavedServiceProvider<ISingleBoundaryAspectService, SingleBoundaryAspectService> _sAspectServiceProvider;
        private readonly WeavedServiceProvider<IMultipleBoundaryAspectsService, MultipleBoundaryAspectsService> _mAspectServiceProvider;
        private readonly WeavedServiceProvider<ITopLevelBoundaryService, TopLevelBoundaryService> _tAspectServiceProvider;
        private readonly WeavedServiceProvider<IInterfaceExtensionService, InterfaceExtensionService> _sInterfaceExtensionServiceProvider;
        private readonly WeavedServiceProvider<IInterfaceExtensionService2, InterfaceExtensionService2> _sInterfaceExtensionService2Provider;
        private readonly WeavedServiceProvider<IRethrowExceptionService, RethrowExceptionService> _sRethrowServiceProvider;

        public MethodBoundaryAspectsTests()
        {
            _sAspectServiceProvider = new WeavedServiceProvider<ISingleBoundaryAspectService, SingleBoundaryAspectService>(new NullComponentsStore());
            
            _mAspectServiceProvider = new WeavedServiceProvider<IMultipleBoundaryAspectsService, MultipleBoundaryAspectsService>(new NullComponentsStore());
            
            _tAspectServiceProvider = new WeavedServiceProvider<ITopLevelBoundaryService, TopLevelBoundaryService>(new NullComponentsStore());
            
            _sInterfaceExtensionServiceProvider = new WeavedServiceProvider<IInterfaceExtensionService, InterfaceExtensionService>(new NullComponentsStore());

            _sInterfaceExtensionService2Provider = new WeavedServiceProvider<IInterfaceExtensionService2, InterfaceExtensionService2>(new NullComponentsStore());

            _sRethrowServiceProvider = new WeavedServiceProvider<IRethrowExceptionService, RethrowExceptionService>(new NullComponentsStore());

            ObservableBoundaryAspect.ClearCallings();
        }

        /// <summary>
        /// Выполняет проверку, что при нормальном выполнении метода
        /// вызываются обработчиет OnEntry, OnSuccess, OnExit.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_BoundariesCalled(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);

            // Act
            service.BypassEmptyMethod();
            
            // Assert
            AspectAssert.OnEntryCalled(typeof(BypassAspect));
            AspectAssert.OnSuccessCalled(typeof(BypassAspect));
            AspectAssert.OnExitCalled(typeof(BypassAspect));
        }

        /// <summary>
        /// Выполняет проверку, что при нормальном выполнении метода, обработчики
        /// OnSuccess и OnExit могут изменять результат выполнения метода.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ReturnValueChangesBy_Exit_And_Success_Boundaries(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(5);
            var context = ObservableBoundaryAspect.GetContext(typeof(IncrementValueAspect), BoundaryType.Exit);
            
            // Assert           
            Assert.Equal(7, context.Source.ReturnValue);
            Assert.Equal(7, result);
        }

        /// <summary>
        /// Выполняет проверку, что обработчик Entry не может изменить результат метода при обычном выполнении.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ReturnValueNotChanged_By_Entry_Boundary(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
            
            // Act
            var result = service.Identity2(5);
         
            // Assert           
            Assert.Equal(5, result);
        }

        /// <summary>
        /// Выполняет проверку, что обработчик Exception может завершить пайплайн с пустым результатом
        /// при этом исключение не будет выброшено наверх.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_EmptyMethod_SwallowException_NotThrowed(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            service.ExceptionalEmptyMethod();
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
        }
        
        /// <summary>
        /// Выполняет проверку, что обработчик Exception может завершить пайплайн с пустым результатом
        /// при этом исключение не будет выброшено наверх.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ValueReturnMethod_SwallowException_DefaultResult(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            var result = service.ExceptionalIdentity(120);
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
            Assert.Equal(default(int), result);
        }

        /// <summary>
        /// Выполняет проверку, что обработчик Exception может завершить пайплайн с пустым результатом
        /// при этом исключение не будет выброшено наверх.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_RefReturnMethod_SwallowException_NullResult(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            var result = service.ExceptionalRef();
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
            Assert.Null(result);
        }
        
        /// <summary>
        /// Выполняет проверку, что обработчик Exception может завершить пайплайн с установленным результатом
        /// при этом исключение не будет выброшено наверх.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ValueReturnMethod_SwallowException_SetResult(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            var result = service.ExceptionalIdentity2(120);
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspect42Result));
            Assert.Equal(42, result);
        }
        
        /// <summary>
        /// Выполняет проверку, что обработчик Exception может завершить пайплайн с установленным результатом
        /// при этом исключение не будет выброшено наверх.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_RefReturnMethod_SwallowException_SetResult(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            var result = service.ExceptionalRef2();
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectNewObjectResult));
            Assert.NotNull(result);
        }

        /// <summary>
        /// Выполняет проверку, что пустой обработчик ошибок не предотвращает прокидывание исключения.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_BypassAspect_NotSwallowException(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            Assert.Throws<Exception>(() => service.ExceptionalEmptyMethod2());
            
            AspectAssert.OnExceptionCalled(typeof(BypassAspect));
        }

        /// <summary>
        /// Выполняет проверку, что обработчик ошибок может заменить исключение на свое.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ReplaceExceptionAspect_NewExceptionThrowed(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ExceptionalEmptyMethod3());
            
            AspectAssert.OnExceptionCalled(typeof(ThrowExceptionAspect));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspectTypeLevel_InterfaceExtend_BaseInterfaceMethods_TriggerOnException(ServiceStoreType storeType)
        {
            // Arrange 
            var service = _sInterfaceExtensionServiceProvider.GetService(storeType);

            // Act & Assert
            service.DoSomethingBase();

            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspectMethodLevel_InterfaceExtend_BaseInterfaceMethods_TriggerOnException(ServiceStoreType storeType)
        {
            // Arrange 
            var service = _sInterfaceExtensionService2Provider.GetService(storeType);

            // Act & Assert
            Assert.Throws<Exception>(() => service.SuppressedMethod());

            var result = service.DoSomethingChild();
            service.DoSomethingBase();

            Assert.Equal(42, result);
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspect42Result));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_RethrowException_ThrowsCorrectException(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sRethrowServiceProvider.GetService(storeType);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.RethrowArgumentException());

            AspectAssert.OnExceptionCalled(typeof(RethrowExceptionAspect));
        }

        /// <summary>
        /// Выполняет проверку, что несколько обработчиков выполняются в порядке убывания приоритета.
        /// </summary>
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_AspectsExecutedInAscOrder(ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(3);

            // Assert           
            Assert.Equal(10, result);
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspectsApplied(ServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity4(3);

            // Assert           
            Assert.Equal(4, result);
        }
        
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_SuppressWeaveAttribute_PreventsRootBoundaryExecuting(ServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity2(3);

            // Assert           
            Assert.Equal(3, result);
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspect_With_SingleMethodAspect(ServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(2);

            // Assert           
            Assert.Equal(9, result);
        }
        
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspect_With_MultipleMethodAspect(ServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity3(2);

            // Assert           
            Assert.Equal(5, result);
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_ExceptionFlow_If_ExceptionThrowedOnSuccess_It_Will_NotHandled_BySelf_Or_Outer_Aspect(ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act & Assert      
            Assert.Throws<Exception>(() => service.IdentityThrowOnSuccess(20));
        }
        
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_ExceptionFlow_If_ExceptionThrowedOnEntry_It_Will_NotHandled_BySelf_Or_Outer_Aspect(ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act & Assert      
            Assert.Throws<Exception>(() => service.IdentityThrowOnEntry(20));
        }
        
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_ExceptionFlow_If_ExceptionThrowedOnExit_It_Will_NotHandled_BySelf_Or_Outer_Aspect(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act & Assert      
            Assert.Throws<Exception>(() => service.IdentityThrowOnExit(20));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_ExceptionFlow_If_ExceptionSwallowed_ByInnerHandler_OuterOnSuccess_Called(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act  
            var result = service.IdentityInnerSuccess(10);
            
            // Assert
            Assert.Equal(1, result);
            
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspectDefaultReturn));
            AspectAssert.OnSuccessNotCalled(typeof(SwallowExceptionAspectDefaultReturn));
            AspectAssert.OnSuccessCalled(typeof(BypassAspect));
            AspectAssert.OnSuccessCalled(typeof(IncrementValueAspect));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_If_BoundaryCallsReturn_Inner_OnSuccess_NotCalled_ButOuter_Does(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act  
            var result = service.IdentityReturnOnEntry(20);
            
            // Assert
            Assert.Equal(43, result);
            
            AspectAssert.OnEntryCalled(typeof(Return42Aspect));
            AspectAssert.OnSuccessNotCalled(typeof(Return42Aspect));
            AspectAssert.OnExitNotCalled(typeof(Return42Aspect));
            
            AspectAssert.OnEntryNotCalled(typeof(BypassAspect));
            AspectAssert.OnSuccessNotCalled(typeof(BypassAspect));
            AspectAssert.OnExitNotCalled(typeof(BypassAspect));
            
            AspectAssert.OnEntryCalled(typeof(IncrementValueAspect));
            AspectAssert.OnSuccessCalled(typeof(IncrementValueAspect));
            AspectAssert.OnExitCalled(typeof(IncrementValueAspect));
        }
        
        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_ExceptionFlow_If_BoundaryThrowPipeline_NextAspect_NotInvoked_ButPrevious_CallsOnExit(
                ServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act  
            Assert.Throws<Exception>(() => service.IdentityThrowPipelineOnEntry(20));
            
            // Assert
            AspectAssert.OnEntryNotCalled(typeof(Return42Aspect));
            AspectAssert.OnExitNotCalled(typeof(Return42Aspect));
            AspectAssert.OnSuccessNotCalled(typeof(Return42Aspect));
            AspectAssert.OnExceptionNotCalled(typeof(Return42Aspect));
            
            AspectAssert.OnEntryCalled(typeof(PipelineThrowAspect));
            AspectAssert.OnExitNotCalled(typeof(PipelineThrowAspect));
            AspectAssert.OnSuccessNotCalled(typeof(PipelineThrowAspect));
            AspectAssert.OnExceptionNotCalled(typeof(PipelineThrowAspect));
            
            AspectAssert.OnEntryCalled(typeof(BypassAspect));
            AspectAssert.OnExitCalled(typeof(BypassAspect));
            AspectAssert.OnSuccessNotCalled(typeof(BypassAspect));
            AspectAssert.OnExceptionNotCalled(typeof(BypassAspect));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_RethrowFlow_RethrowOnException_Handled_InOnException_HigherOrder_Aspect(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _sRethrowServiceProvider.GetService(storeType);

            // Act
            service.RethrowArgumentExceptionThenSwallow();

            // Assert
            AspectAssert.OnExceptionCalled(typeof(RethrowExceptionAspect));
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionIfTypeMatchAspect));
        }

        [Theory]
        [InlineData(ServiceStoreType.TransientWeaving)]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void MultipleAspects_RethrowFlow_RethrowOnEntry_Handled_InOnException_HigherOrder_Aspect(
            ServiceStoreType storeType)
        {
            // Arrange
            var service = _sRethrowServiceProvider.GetService(storeType);

            // Act
            var result = service.RehrowOnEntryThenSwallowReturn42();

            // Assert
            AspectAssert.OnEntryCalled(typeof(RethrowExceptionAspect));
            AspectAssert.OnExceptionCalled(typeof(SwallowExceptionAspect42Result));

            Assert.Equal(42, result);
        }

        [Theory]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ServiceProvider_InjectInstance_Correct(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
            
            // Act
            var result = service.Identity3(5);
         
            // Assert           
            Assert.Equal(10, result);
        }

        [Theory]
        [InlineData(ServiceStoreType.CastleWindsor)]
        [InlineData(ServiceStoreType.SimpleInjector)]
        public void AspectDependency_WithPublicGetSet_Works_ServiceInjected(ServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
            
            // Act
            var result = service.DependencyIdentity(5);
         
            // Assert           
            AspectAssert.OnEntryCalled(typeof(DependencyAspect));
            AspectAssert.OnSuccessCalled(typeof(DependencyAspect));
            AspectAssert.OnExitCalled(typeof(DependencyAspect));
        }
    }
}