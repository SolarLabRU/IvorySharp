using System;
using System.Reflection;

namespace IvorySharp.Proxying
{
    /// <summary>
    /// Базовый класс прокси для .NET Core.
    /// Сворован из corefx репозитория.
    /// https://github.com/dotnet/corefx/blob/master/src/System.Reflection.DispatchProxy/src/System/Reflection/DispatchProxy.cs
    /// </summary>
    public abstract class DispatchProxy
    {
        protected DispatchProxy() { }
        
        /// <summary>
        /// Whenever any method on the generated proxy type is called, this method
        /// will be invoked to dispatch control.
        /// </summary>
        /// <param name="targetMethod">The method the caller invoked</param>
        /// <param name="args">The arguments the caller passed to the method</param>
        /// <returns>The object to return to the caller, or <c>null</c> for void methods</returns>
        protected abstract object Invoke(MethodInfo targetMethod, object[] args);
        
        /// <summary>
        /// Creates an object instance that derives from class <typeparamref name="TProxy"/>
        /// and implements interface <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The interface the proxy should implement.</typeparam>
        /// <typeparam name="TProxy">The base class to use for the proxy class.</typeparam>
        /// <returns>An object instance that implements <typeparamref name="T"/>.</returns>
        /// <exception cref="System.ArgumentException"><typeparamref name="T"/> is a class, 
        /// or <typeparamref name="TProxy"/> is sealed or does not have a parameterless constructor</exception>
        protected static T CreateTrasparentProxy<T, TProxy>()
            where TProxy : DispatchProxy
        {
            return (T)DispatchProxyGenerator.CreateProxyInstance(typeof(TProxy), typeof(T));
        }

        protected static object CreateTrasparentProxy<TProxy>(Type targetDeclaredType)
            where TProxy : DispatchProxy
        {
            return DispatchProxyGenerator.CreateProxyInstance(typeof(TProxy), targetDeclaredType);
        }
    }
}