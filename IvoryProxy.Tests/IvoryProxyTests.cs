using System;
using IvoryProxy.Core;
using IvoryProxy.Core.Attributes;
using IvoryProxy.Core.Exceptions;
using IvoryProxy.Tests.Interceptors;
using Xunit;

namespace IvoryProxy.Tests
{
    /// <summary>
    /// Набор общих тестов на то, что либа работает.
    /// </summary>
    public class IvoryProxyTests
    {
        [Fact]
        public void Intercept_Interface_InterceptedMethod_CallsTo_Interceptor()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { proxy.ThrowArgumentExceptionIfIntercepted(); });
        }

        [Fact]
        public void Intercept_Interface_InterceptMethod_AffectResult()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.InterceptedIncrementIdentity(2);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void Intercept_Interface_NotDecoratedMethod_NotIntercepted()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.NotInterceptedIdentity(2);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Intercept_Interface_Swallows_Exception_ReturnsDefaultValueType()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.ExceptionSwallowedReturnsInt();

            // Assert
            Assert.Equal(default(int), result);
        }

        [Fact]
        public void Intercept_Interface_Swallows_Exception_ReturnsVoid()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.ExceptionSwallowedReturnsNull();

            // Assert
            Assert.Equal(default(object), result);
        }

        [Fact]
        public void Intercept_Interface_Swallows_Exception_ReturnsDefaultRefType()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            proxy.ExceptionSwallowedReturnsVoid();
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Void_NotThrows()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            proxy.ForgotSetReturnValueVoid();
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Int_Throws()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act

            Assert.Throws<IvoryProxyException>(() => { proxy.ForgotSetReturnValueInt(2); });
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Object_Throws()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<ISpecificMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            Assert.Throws<IvoryProxyException>(() => { proxy.ForgotSetReturnValueObject(); });
        }

        [Fact]
        public void Intercept_Interface_GlobalIntercepter_Applied_To_MultipleMethods()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IGlobalMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result1 = proxy.InterceptorAppliedIdentity1(2);
            var result2 = proxy.InterceptorAppliedIdentity2(2);

            // Assert
            Assert.Equal(3, result1);
            Assert.Equal(3, result2);
        }

        [Fact]
        public void Intercept_Intefrace_GlobalIntercepted_NotAppied_To_MethodWithDissalowIntercept()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IGlobalMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.DisallowedIdentity(2);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Intercept_Interface_GlobalIntercepted_NotApplied_To_MethodWithNotAcceptableSignature()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IGlobalMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.NotAcceptableIdentity(2);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void Intercept_Interface_DuplicateIntercepted_NotApplied()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IGlobalMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.IdenityWithDuplicateIncrementInterceptor(2);

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public void Intercept_Interface_GlobalAndLocalInterceptors_AppliedMostPrecise()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IGlobalMethodInterceptionService>(new DoNothingService()).TransparentProxy;

            // Act
            var result = proxy.TwoIncrementInterceptorsIdentity(3);

            // Assert
            Assert.Equal(9, result);
        }

        [Fact]
        public void Intercept_Class_Throws()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();

            // Act & Assert
            Assert.Throws<IvoryProxyException>(() =>
            {
                var proxy = proxyGenerator.CreateInterfaceProxy<DoNothingService>(new DoNothingService());
            });
        }
    }

    public interface ISpecificMethodInterceptionService
    {
        [Intercept(typeof(ThrowExceptionInterceptor<ArgumentException>))]
        void ThrowArgumentExceptionIfIntercepted();

        int NotInterceptedIdentity(int argument);

        [Intercept(typeof(IncrementResultInterceptor))]
        int InterceptedIncrementIdentity(int argument);

        [Intercept(typeof(SwallowExceptionInterceptor))]
        int ExceptionSwallowedReturnsInt();

        [Intercept(typeof(SwallowExceptionInterceptor))]
        void ExceptionSwallowedReturnsVoid();

        [Intercept(typeof(SwallowExceptionInterceptor))]
        object ExceptionSwallowedReturnsNull();

        [Intercept(typeof(ForgotSetReturnValueInterceptor))]
        void ForgotSetReturnValueVoid();

        [Intercept(typeof(ForgotSetReturnValueInterceptor))]
        int ForgotSetReturnValueInt(int argument);

        [Intercept(typeof(ForgotSetReturnValueInterceptor))]
        object ForgotSetReturnValueObject();
    }

    [Intercept(typeof(IncrementResultInterceptor))]
    public interface IGlobalMethodInterceptionService
    {
        int InterceptorAppliedIdentity1(int argument);

        int InterceptorAppliedIdentity2(int argument);

        [Intercept(typeof(PowTwoResultInterceptor))]
        int TwoIncrementInterceptorsIdentity(int argument);

        [DisallowIntercept]
        int DisallowedIdentity(int argument);

        [Intercept(typeof(IncrementResultInterceptor))]
        int IdenityWithDuplicateIncrementInterceptor(int argument);

        short NotAcceptableIdentity(short argument);
    }

    public class DoNothingService : ISpecificMethodInterceptionService, IGlobalMethodInterceptionService
    {
        public void ThrowArgumentExceptionIfIntercepted()
        {
        }

        public int NotInterceptedIdentity(int argument)
        {
            return argument;
        }

        public int InterceptedIncrementIdentity(int argument)
        {
            return argument;
        }

        public int ExceptionSwallowedReturnsInt()
        {
            throw new ArgumentException();
        }

        public void ExceptionSwallowedReturnsVoid()
        {
            throw new ArgumentException();
        }

        public object ExceptionSwallowedReturnsNull()
        {
            throw new ArgumentException();
        }

        public void ForgotSetReturnValueVoid()
        {
        }

        public int ForgotSetReturnValueInt(int argument)
        {
            return argument;
        }

        public object ForgotSetReturnValueObject()
        {
            return new object();
            ;
        }

        public int InterceptorAppliedIdentity1(int argument)
        {
            return argument;
        }

        public int InterceptorAppliedIdentity2(int argument)
        {
            return argument;
        }

        public int TwoIncrementInterceptorsIdentity(int argument)
        {
            return argument;
        }

        public int DisallowedIdentity(int argument)
        {
            return argument;
        }

        public int IdenityWithDuplicateIncrementInterceptor(int argument)
        {
            return argument;
        }

        public short NotAcceptableIdentity(short argument)
        {
            return argument;
        }
    }
}