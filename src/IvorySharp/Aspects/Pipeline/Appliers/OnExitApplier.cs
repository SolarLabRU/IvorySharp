namespace IvorySharp.Aspects.Pipeline.Appliers
{
    /// <summary>
    /// Выполняет точку прикрепления <see cref="MethodBoundaryAspect.OnExit"/>.
    /// </summary>
    internal class OnExitApplier : AspectApplier
    {
        /// <summary>
        /// Инициализированный экземпляр <see cref="OnSuccessApplier"/>.
        /// </summary>
        public static readonly OnExitApplier Instance = new OnExitApplier();
        
        private OnExitApplier() { }

        /// <inheritdoc />
        public override bool CanApply(IInvocationPipeline pipeline)
        {
            return pipeline.FlowBehavior != FlowBehavior.Faulted;
        }

        /// <inheritdoc />
        public override AspectApplyResult Apply(MethodBoundaryAspect aspect, IInvocationPipeline pipeline)
        {
            aspect.OnExit(pipeline);
            
            var shouldBreak = pipeline.FlowBehavior == FlowBehavior.Faulted;
            
            return new AspectApplyResult(shouldBreak ? aspect : null);
        }
    }
}