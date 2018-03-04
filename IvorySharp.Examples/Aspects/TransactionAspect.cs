﻿using System.Transactions;
using IvorySharp.Aspects;
using IvorySharp.Aspects.Pipeline;

namespace IvorySharp.Examples.Aspects
{
    /// <summary>
    /// Устанавливает выполнение действия в рамках транзакции.
    /// </summary>
    public class TransactionAspect : MethodBoundaryAspect
    {
        /// <inheritdoc />
        public override void OnEntry(IInvocationPipeline pipeline)
        {
            var ts = new TransactionScope(TransactionScopeOption.Required);
            pipeline.AspectExecutionState = ts;
        }

        /// <inheritdoc />
        public override void OnSuccess(IInvocationPipeline pipeline)
        {
            var ts = (TransactionScope) pipeline.AspectExecutionState;
            ts.Complete();
        }

        /// <inheritdoc />
        public override void OnExit(IInvocationPipeline pipeline)
        {
            var ts = (TransactionScope) pipeline.AspectExecutionState;
            ts.Dispose();
        }
    }
}