using System;
using System.Linq;
using System.Reflection;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Components;
using IvorySharp.Aspects.Selection;
using IvorySharp.Aspects.Weaving;
using Moq;
using Xunit;

namespace IvorySharp.Tests.UnitTests
{
    /// <summary>
    /// Набор тестов для <see cref="DeclaringTypeWeavePredicate"/>.
    /// </summary>
    public class DeclaringTypeWeavePredicateTests
    {
        /// <summary>
        /// Сценарий: На интерфейсе и методах нет аспектов
        /// Ожидамый результат: Применение обвязки невозможно
        /// Почему: Нет смысла создавать прокси и замедлять производительность компонента, если вызовы к методам не будут перехвачены.
        /// </summary>
        [Fact]
        public void ZeroAspects_IsWeaveable_TypeOverload_Returns_False()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<MethodInfo>(), It.IsAny<bool>())).Returns(false);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<Type>(), It.IsAny<bool>())).Returns(false);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IService), typeof(Service));

            // Assert
            Assert.False(isWeaveable);
        }

        /// <summary>
        /// Сценарий: Объявленный тип не является интерфейсом
        /// Ожидамый результат: Применение обвязки невозможно
        /// Почему: Проксирование возможно только для интерфейсов
        /// </summary>
        [Fact]
        public void DeclaringTypeNotInterface_IsWeveable_TypeOverload_Returns_False()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();
            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(Service), typeof(IService));

            // Assert
            Assert.False(isWeaveable);
        }

        /// <summary>
        /// Сценарий: Объявленный тип помечен атрибутом, запрещающим применение аспектов
        /// Ожидамый результат: Применение обвязки невозможно
        /// </summary>
        [Fact]
        public void SuppressedInterface_IsWeaveable_TypeOverload_ReturnsFalse()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<MethodInfo>(), It.IsAny<bool>())).Returns(true);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<Type>(), It.IsAny<bool>())).Returns(true);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IWeavingSuppressedService), typeof(WeavingSuppressedService));

            // Assert
            Assert.False(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На объявленном типе есть аспекты
        /// Ожидамый результат: Применение обвязки возможно
        /// </summary>
        [Fact]
        public void HasTypeAspects_IsWeaveable_TypeOverload_ReturnsTrue()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<MethodInfo>(), It.IsAny<bool>())).Returns(false);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<Type>(), It.IsAny<bool>())).Returns(true);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IService), typeof(Service));

            // Assert
            Assert.True(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На методах объявленного типа есть аспекты
        /// Ожидамый результат: Применение обвязки возможно
        /// </summary>
        [Fact]
        public void HasMethodAspects_IsWeaveable_TypeOverload_ReturnsTrue()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<MethodInfo>(), It.IsAny<bool>())).Returns(true);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<Type>(), It.IsAny<bool>())).Returns(false);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());
            
            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IService), typeof(Service));

            // Assert
            Assert.True(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На методах базового интерфейса есть аспекты
        /// Ожидамый результат: Применение обвязки возможно
        /// </summary>
        [Fact]
        public void HasMethodAspects_InBaseInterface_IsWeaveable_TypeOverload_ReturnsTrue()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<MethodInfo>(), It.IsAny<bool>())).Returns(true);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.Is<Type>(t => t.GetInterfaces().Contains(typeof(IService))), It.IsAny<bool>()))
                .Returns(false);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IServiceExtend), typeof(Service));

            // Assert
            Assert.True(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На базовом интерфейса есть аспекты
        /// Ожидамый результат: Применение обвязки возможно
        /// </summary>
        [Fact]
        public void HasTypeAspects_InBaseInterface_IsWeaveable_TypeOverload_ReturnsTrue()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.Is<MethodInfo>(m => m.DeclaringType == typeof(IService)), It.IsAny<bool>())).Returns(true);
            selectionStategyMock.Setup(c => c.HasAnyAspect(It.IsAny<Type>(), It.IsAny<bool>())).Returns(false);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IServiceExtend), typeof(Service));

            // Assert
            Assert.True(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На методе есть атрибут запрещающий применение аспектов
        /// Ожидамый результат: Применение обвязки невозможно
        /// </summary>
        [Fact]
        public void HasAnyAspect_MethodSupressed_IsWeaveable_MethodOverload_ReturnsFalse()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();
            var method = typeof(IService).GetMethod(nameof(IService.SupressedMethod));

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.Is<MethodInfo>(m => m == method), It.IsAny<bool>())).Returns(true);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IService), typeof(Service));

            // Assert
            Assert.False(isWeaveable);
        }

        /// <summary>
        /// Сценарий: На методе есть аспект
        /// Ожидамый результат: Применение обвязки возможно
        /// </summary>
        [Fact]
        public void HasAnyAspect_MethodHasAspect_IsWeaveable_MethodOverload_ReturnsTrue()
        {
            // Arrange
            var selectionStategyMock = new Mock<IAspectSelector>();
            var method = typeof(IService).GetMethod(nameof(IService.Method));

            selectionStategyMock.Setup(c => c.HasAnyAspect(It.Is<MethodInfo>(m => m == method), It.IsAny<bool>())).Returns(true);

            var predicate = new DeclaringTypeWeavePredicate(selectionStategyMock.Object.ToProvider());

            // Act
            var isWeaveable = predicate.IsWeaveable(typeof(IService), typeof(Service));

            // Assert
            Assert.True(isWeaveable);
        }

        #region Services

        private interface IService
        {
            void Method();

            [SuppressAspectsWeaving]
            void SupressedMethod();
        }

        private interface IServiceExtend : IService
        {
            
        }
        
        private class Service : IServiceExtend
        {
            public void Method()
            {
                
            }

            public void SupressedMethod()
            {
                
            }
        }

        [SuppressAspectsWeaving]
        private interface IWeavingSuppressedService
        {
        }

        private class WeavingSuppressedService : IWeavingSuppressedService
        {
            
        }

        #endregion
    }
}