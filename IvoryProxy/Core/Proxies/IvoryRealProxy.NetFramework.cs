namespace IvoryProxy.Core.Proxies
{
    #if NET461
    
    /// <summary>
    /// Реализация динамического прокси на основе <see cref="RealProxy"/>.
    /// </summary>
    /// <typeparam name="T">Тип объекта для проксирования.</typeparam>
    public class IvoryRealProxy<T> : IIvoryProxy<T> 
        where T : class 
    {
        /// <inheritdoc />
        public T Decorated { get; }

        /// <inheritdoc />
        public T Proxy { get; }

        /// <inheritdoc />
        public void Proceed(IMethodInvocation invocation)
        {
            throw new System.NotImplementedException();
        }
    }
    
    #endif
}