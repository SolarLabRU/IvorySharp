namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Выполняет точку прикрепления <see cref="MethodBoundaryAspect.OnSuccess"/>.
    /// </summary>
    internal class OnSuccessApplier : AspectApplier
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="OnSuccessApplier"/>.
        /// </summary>
        public static readonly OnSuccessApplier Instance = new OnSuccessApplier();

        private OnSuccessApplier()
        {
        }

        /// <inheritdoc />
        public override bool CanApply(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior == FlowBehavior.Default ||
                   pipeline.FlowBehavior == FlowBehavior.Return;
        }

        /// <inheritdoc />
        public override AspectApplyResult Apply(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnSuccess(pipeline);

            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.ThrowException ||
                              pipeline.FlowBehavior == FlowBehavior.RethrowException ||
                              pipeline.FlowBehavior == FlowBehavior.Faulted;

            return new AspectApplyResult(shouldBreak ? aspect : null);
        }
    }
}