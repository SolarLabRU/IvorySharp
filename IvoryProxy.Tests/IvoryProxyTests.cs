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
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => { proxy.ThrowArgumentExceptionIfIntercepted(); });
        }

        [Fact]
        public void Intercept_Interface_InterceptMethod_AffectResult()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

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
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

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
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

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
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

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
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

            // Act
            proxy.ExceptionSwallowedReturnsVoid();
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Void_NotThrows()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

            // Act
            proxy.ForgotSetReturnValueVoid();
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Int_Throws()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

            // Act

            Assert.Throws<IvoryProxyException>(() => { proxy.ForgotSetReturnValueInt(2); });
        }

        [Fact]
        public void Intercept_Intefrace_ForgotSetReturnValue_Object_Throws()
        {
            // Arrange
            var proxyGenerator = new IvoryProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxy<IService>(new DoNothingService()).Proxy;

            // Act
            Assert.Throws<IvoryProxyException>(() => { proxy.ForgotSetReturnValueObject(); });
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

    public interface IService
    {
        [InterceptMethod(typeof(ThrowExceptionInterceptor<ArgumentException>))]
        void ThrowArgumentExceptionIfIntercepted();

        int NotInterceptedIdentity(int argument);

        [InterceptMethod(typeof(IncrementResultInterceptor))]
        int InterceptedIncrementIdentity(int argument);

        [InterceptMethod(typeof(SwallowExceptionInterceptor))]
        int ExceptionSwallowedReturnsInt();

        [InterceptMethod(typeof(SwallowExceptionInterceptor))]
        void ExceptionSwallowedReturnsVoid();

        [InterceptMethod(typeof(SwallowExceptionInterceptor))]
        object ExceptionSwallowedReturnsNull();

        [InterceptMethod(typeof(ForgotSetReturnValueInterceptor))]
        void ForgotSetReturnValueVoid();

        [InterceptMethod(typeof(ForgotSetReturnValueInterceptor))]
        int ForgotSetReturnValueInt(int argument);

        [InterceptMethod(typeof(ForgotSetReturnValueInterceptor))]
        object ForgotSetReturnValueObject();
    }

    public class DoNothingService : IService
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
    }
}