using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using IvoryProxy.Helpers;

namespace IvoryProxy.Core.Interceptors
{
    /// <summary>
    /// Базовый класс перехватчика вызовов методов с заменой обработки исключений.
    /// </summary>
    public abstract class ExceptionInterceptor : IInterceptor
    {
        /// <summary>
        /// Обработчик исключений.
        /// </summary>
        /// <param name="exception">Исключение.</param>
        protected abstract void OnException(Exception exception);

        /// <summary>
        /// Возвращает признак того, что ислкючение может быть поглощено без дальнейшей проброски вверх.
        /// </summary>
        /// <param name="exception">Исходное исключение.</param>
        /// <returns>Признак того, что исключение может быть поглощено.</returns>
        protected abstract bool CanSwallowException(Exception exception);
        
        /// <inheritdoc />
        public void Intercept(IMethodInvocation invocation)
        {
            try
            {
                invocation.Proceed();

                // TODO[IP] Написать тесты на это и разобраться нужно ли вообще перехватывать исключения в задачах
                if (invocation.IsReturnVoid && invocation.ReturnValue is Task taskResult)
                {
                    taskResult.ContinueWith(task =>
                    {
                        task.Exception.Handle(e =>
                        {
                            OnException(e.InnerException);
                            return CanSwallowException(e);
                        });
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException te && te.InnerException != null)
                {
                    OnException(te.InnerException);

                    if (!CanSwallowException(e))
                        ExceptionDispatchInfo.Capture(te.InnerException).Throw();
                }
                else
                {
                    OnException(e);
                    
                    if (!CanSwallowException(e))
                        throw;
                }
            }

            invocation.TrySetReturnValue(ValueHelper.GetDefault(invocation.TargetMethod.ReturnType));
        }

        /// <inheritdoc />
        public virtual bool CanIntercept(IMethodPreExecutionContext context)
        {
            return true;
        }
    }
}