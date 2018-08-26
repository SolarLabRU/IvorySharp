using System;
using System.Threading;
using IvorySharp.Aspects;
using IvorySharp.Core;

namespace Examples.AutoRetry
{
    /// <summary>
    /// Выполняет повторную попытку вызова метода в случае исключения.
    /// </summary>
    public sealed class RetryAspectAttribute : MethodInterceptionAspect
    {
        /// <summary>
        /// Количество попыток.
        /// </summary>
        public int RetriesCount { get; set; }
        
        /// <summary>
        /// Задержка перед следующим вызовом.
        /// </summary>
        public int DelayMs { get; set; }

        /// <inheritdoc />
        public override void OnInvoke(IInvocation invocation)
        {
            for(int attempt = 0;; attempt++)
            {
                try
                {
                    invocation.Proceed();
                    
                    Console.WriteLine($"Invocation successfull. Return value: {invocation.ReturnValue}." +
                                      $" Attempts {attempt} of {RetriesCount}");
                    
                    return;
                }
                catch (Exception e)
                {
                    if (attempt < RetriesCount)
                    {
                        Console.WriteLine($"Retry {attempt} of {RetriesCount}. Waiting '{DelayMs}' ms...");
                        Thread.Sleep(DelayMs);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}