namespace IvorySharp.Aspects
{
    /// <summary>
    /// Аспект, который ничего не делает.
    /// </summary>
    internal class NullMethodBoundaryAspect : MethodBoundaryAspect
    {
        /// <summary>
        /// Экземпляр аспекта.
        /// </summary>
        internal static readonly NullMethodBoundaryAspect Instance = new NullMethodBoundaryAspect();

        private NullMethodBoundaryAspect() { }
    }
}