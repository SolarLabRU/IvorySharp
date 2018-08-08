namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Выполняет точку прикрепления <see cref="MethodBoundaryAspect.OnException"/>.
    /// </summary>
    internal class OnExceptionApplier : AspectApplier
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="OnExceptionApplier"/>.
        /// </summary>
        public static readonly OnExceptionApplier Instance = new OnExceptionApplier();

        private OnExceptionApplier()
        { }

        /// <inheritdoc />
        public override bool CanApply(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.RethrowException;
        }

        /// <inheritdoc />
        public override AspectApplyResult Apply(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnException(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.Return ||
                              pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                              pipeline.FlowBehavior == FlowBehavior.Faulted;

            return new AspectApplyResult(shouldBreak ? aspect : null);
        }
    }
}