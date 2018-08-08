namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Базовый класс компонента, применяющего аспект к пайлайну.
    /// </summary>
    internal abstract class AspectApplier
    {
        /// <summary>
        /// Выполняет проверку возможности выполнения аспекта.
        /// </summary>
        /// <param name="pipeline">Модель пайплайна.</param>
        /// <returns>Признак возможности выполнения аспекта.</returns>
        public abstract bool CanApply(IInvocationPipeline pipeline);
        
        /// <summary>
        /// Выполняет аспект <paramref name="aspect"/> на пайплайне <paramref name="pipeline"/>.
        /// </summary>
        /// <param name="aspect">Аспект для применения.</param>
        /// <param name="pipeline">Пайплайн.</param>
        /// <returns>Результат применения аспекта.</returns>
        public abstract AspectApplyResult Apply(MethodBoundaryAspect aspect, IInvocationPipeline pipeline);
    }
}