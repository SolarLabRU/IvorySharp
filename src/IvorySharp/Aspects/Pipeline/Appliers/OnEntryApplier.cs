namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Выполняет точку прикрепления <see cref="MethodBoundaryAspect.OnEntry"/>.
    /// </summary>
    internal class OnEntryApplier : AspectApplier
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="OnEntryApplier"/>.
        /// </summary>
        public static readonly OnEntryApplier Instance = new OnEntryApplier();

        private OnEntryApplier()
        {
        }

        /// <inheritdoc />
        public override bool CanApply(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior != FlowBehavior.Faulted &&
                   pipeline.FlowBehavior != FlowBehavior.ThrowException &&
                   pipeline.FlowBehavior != FlowBehavior.RethrowException &&
                   pipeline.FlowBehavior != FlowBehavior.Return;
        }

        /// <inheritdoc />
        public override AspectApplyResult Apply(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnEntry(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.RethrowException ||
                              pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                              pipeline.FlowBehavior == FlowBehavior.Return ||
                              pipeline.FlowBehavior == FlowBehavior.Faulted;

            return new AspectApplyResult(shouldBreak ? aspect : null);
        }
    }
}