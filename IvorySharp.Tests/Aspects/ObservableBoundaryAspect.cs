using System;
using System.Collections.Generic;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;
using IvorySharp.Tests.Helpers;

namespace IvorySharp.Tests.Aspects
{
    /// <summary>
    /// Позволяет отслеживать, что шаги были вызваны.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public abstract class ObservableBoundaryAspect : MethodBoundaryAspect
    {
        private static Dictionary<Type, AspectBoundaryCalling> _callings;

        static ObservableBoundaryAspect()
        {
            _callings = new Dictionary<Type, AspectBoundaryCalling>();
        }

        protected ObservableBoundaryAspect()
        {
            Key = GetType();
        }

        /// <summary>
        /// Уникальный ключ аспекта.
        /// </summary>
        public Type Key { get; }

        public static void ClearCallings()
        {
            _callings.Clear();
        }

        public static ExceptionalContext GetContext(Type aspectType, BoundaryType boundaryType)
        {
            if (!_callings.ContainsKey(aspectType))
                return null;

            var calling = _callings[aspectType];
            
            switch (boundaryType)
            {
                case BoundaryType.Entry:
                    return calling.EntryContext;
                case BoundaryType.Success:
                    return calling.SuccessContext;
                case BoundaryType.Exit:
                    return calling.ExitContext;
                case BoundaryType.Exception:
                    return calling.ExceptionContext;
    
                default:
                    throw new ArgumentOutOfRangeException(nameof(boundaryType), boundaryType, null);
            }
        }

        public static bool HasContext(Type aspectType, BoundaryType boundaryType)
        {
            return GetContext(aspectType, boundaryType) != null;
        }
        
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            if (!_callings.TryGetValue(Key, out var calling))
            {
                calling = new AspectBoundaryCalling();
                _callings[Key] = calling;
            }

            calling.EntryContext = ExceptionalContext.FromPipeline(pipeline);
            Entry(pipeline);
        }
        
        protected virtual void Entry(IInvocationPipeline pipeline){ }

        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            if (!_callings.TryGetValue(Key, out var calling))
            {
                calling = new AspectBoundaryCalling();
                _callings[Key] = calling;
            }

            calling.SuccessContext = ExceptionalContext.FromPipeline(pipeline);
            
            Success(pipeline);
        }

        protected virtual void Success(IInvocationPipeline pipeline) { }
        
        public override void OnException(IInvocationPipeline pipeline)
        {
            if (!_callings.TryGetValue(Key, out var calling))
            {
                calling = new AspectBoundaryCalling();
                _callings[Key] = calling;
            }

            calling.ExceptionContext = ExceptionalContext.FromPipeline(pipeline);
            
            Exception(pipeline);
        }

        protected virtual void Exception(IInvocationPipeline pipeline) { }
        
        public override void OnExit(IInvocationPipeline pipeline)
        {
            if (!_callings.TryGetValue(Key, out var calling))
            {
                calling = new AspectBoundaryCalling();
                _callings[Key] = calling;
            }

            calling.ExitContext = ExceptionalContext.FromPipeline(pipeline);
            
            Exit(pipeline);
        }
        
        protected virtual void Exit(IInvocationPipeline pipeline){ }
        
        public class AspectBoundaryCalling
        {
            public ExceptionalContext EntryContext { get; set; }
            public ExceptionalContext SuccessContext { get; set; }
            public ExceptionalContext ExitContext { get; set; }
            public ExceptionalContext ExceptionContext { get; set; }
        }
    }
}