using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using IvoryProxy.Extensions;

namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Перехватывает вызов метода, добавляя действия на пре и пост обработку.
    /// </summary>
    public abstract class ActionInterceptor : IInterceptor
    {
        /// <summary>
        /// Вызывается перед выполнением метода.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        public abstract void BeforeExecution(IMethodPreExecutionContext context);

        /// <summary>
        /// Вызывается после выполнения метода.
        /// </summary>
        /// <param name="context">Контекст выполнения метода.</param>
        public abstract void AfterExecution(IMethodPostExecutionContext context);

        /// <summary>
        /// Вызывается в случае, если при выполнении основного метода произошло исключение.
        /// По умолчанию исключение будет прокинуто выше.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        public virtual void OnProceedException(Exception exception)
        {
            RethrowException(exception);
        }

        /// <summary>
        /// Вызывается в случае, если при выполнении действия пре/пост действия произошло исключение.
        /// По умолчанию исключение будет прокинуто выше.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        public virtual void OnActionException(Exception exception)
        {
            RethrowException(exception);
        }
        
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            try
            {
                try
                {
                    BeforeExecution(invocation.ToPreExecutionContext());
                }
                catch (Exception e)
                {
                    OnActionException(e);
                }
                
                invocation.Proceed();
                
                try
                {
                    AfterExecution(invocation.ToPostExecutionContext());
                }
                catch (Exception e)
                {
                    OnActionException(e);
                }
            }
            catch (Exception e)
            {
                var exception = e is TargetInvocationException ei && ei.InnerException != null
                    ? ei.InnerException
                    : e;

                try
                {
                    OnProceedException(exception);
                }
                catch (Exception inner)
                {
                    throw new AggregateException(exception, inner);
                }
            }
        }

        /// <inheritdoc />
        public virtual bool CanIntercept(IMethodInvocation invocation)
        {
            return true;
        }

        /// <summary>
        /// Пробрасывает исключение, сохраняя стек-трейс.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        protected void RethrowException(Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}