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
    public class MethodBoundaryAspectsTests
    {
        private readonly WeavedServiceProvider<ISingleBoundaryAspectService, SingleBoundaryAspectService> _sAspectServiceProvider;
        private readonly WeavedServiceProvider<IMultipleBoundaryAspectsService, MultipleBoundaryAspectsService> _mAspectServiceProvider;
        private readonly WeavedServiceProvider<ITopLevelBoundaryService, TopLevelBoundaryService> _tAspectServiceProvider;
        private readonly WeavedServiceProvider<INotMarkedExplicitBoundaryService, ExplicitBoundaryService> _tnmExplicitServiceProvider;
        private readonly WeavedServiceProvider<IMarkedExplicitBoundaryService, ExplicitBoundaryService> _tmExplicitServiceProvider;
        
        public MethodBoundaryAspectsTests()
        {
            _sAspectServiceProvider = new WeavedServiceProvider<ISingleBoundaryAspectService, SingleBoundaryAspectService>(new ImpliticAspectsWeavingSettings());
            
            _mAspectServiceProvider = new WeavedServiceProvider<IMultipleBoundaryAspectsService, MultipleBoundaryAspectsService>(new ImpliticAspectsWeavingSettings());
            
            _tAspectServiceProvider = new WeavedServiceProvider<ITopLevelBoundaryService, TopLevelBoundaryService>(new ImpliticAspectsWeavingSettings());
            
            _tnmExplicitServiceProvider = new WeavedServiceProvider<INotMarkedExplicitBoundaryService, ExplicitBoundaryService>(new ExplicitAspectWeavingSettings());
            
            _tmExplicitServiceProvider = new WeavedServiceProvider<IMarkedExplicitBoundaryService, ExplicitBoundaryService>(new ExplicitAspectWeavingSettings());
            
            ObservableBoundaryAspect.ClearCallings();
        }

        #region Single Aspect

        /// <summary>
        /// Выполняет проверку, что при нормальном выполнении метода
        /// вызываются обработчиет OnEntry, OnSuccess, OnExit.
        /// </summary>
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_BoundariesCalled(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ReturnValueChangesBy_Exit_And_Success_Boundaries(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ReturnValueNotChanged_By_Entry_Boundary(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_EmptyMethod_SwallowException_NotThrowed(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ValueReturnMethod_SwallowException_DefaultResult(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_RefReturnMethod_SwallowException_NullResult(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ValueReturnMethod_SwallowException_SetResult(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_RefReturnMethod_SwallowException_SetResult(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_BypassAspect_NotSwallowException(WeavedServiceStoreType storeType)
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
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_ExceptionFlow_ReplaceExceptionAspect_NewExceptionThrowed(
            WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
      
            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.ExceptionalEmptyMethod3());
            
            AspectAssert.OnExceptionCalled(typeof(ReplaceExceptionAspect));
        }

        /// <summary>
        /// Выполняет проверку, что несколько обработчиков выполняются в порядке убывания приоритета.
        /// </summary>
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_AspectsExecutedInAscOrder(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _mAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(3);

            // Assert           
            Assert.Equal(10, result);
        }

        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspectsApplied(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity4(3);

            // Assert           
            Assert.Equal(4, result);
        }
        
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_SuppressWeaveAttribute_PreventsRootBoundaryExecuting(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity2(3);

            // Assert           
            Assert.Equal(3, result);
        }

        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspect_With_SingleMethodAspect(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(2);

            // Assert           
            Assert.Equal(9, result);
        }
        
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void MultipleAspects_NormalFlow_RootLevelAspect_With_MultipleMethodAspect(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tAspectServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity3(2);

            // Assert           
            Assert.Equal(5, result);
        }
        
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void ExplicitMarkers_AspectsNotApplied_If_Service_NotMarked(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tnmExplicitServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(2);

            // Assert           
            Assert.Equal(2, result);
        }
        
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void ExplicitMarkers_AspectsApplied_If_Service_Marked(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tmExplicitServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity(2);

            // Assert           
            Assert.Equal(3, result);
        }
        
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void ExplicitMarkers_SuppressedAspect_NotApplied_If_Service_NotMarker(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tnmExplicitServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity2(2);

            // Assert           
            Assert.Equal(2, result);
        }
                
        [Theory]
        [InlineData(WeavedServiceStoreType.TransientWeaving)]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void ExplicitMarkers_SuppressedAspect_NotApplied_If_Service_Marker(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _tmExplicitServiceProvider.GetService(storeType);

            // Act
            var result = service.Identity2(2);

            // Assert           
            Assert.Equal(2, result);
        }

        [Theory]
        [InlineData(WeavedServiceStoreType.CastleWindsor)]
        [InlineData(WeavedServiceStoreType.SimpleInjector)]
        public void SingleAspect_NormalFlow_ServiceProvider_GetInstance_Correct(WeavedServiceStoreType storeType)
        {
            // Arrange
            var service = _sAspectServiceProvider.GetService(storeType);
            
            // Act
            var result = service.Identity3(5);
         
            // Assert           
            Assert.Equal(10, result);
        }
        
        #endregion
    }
}