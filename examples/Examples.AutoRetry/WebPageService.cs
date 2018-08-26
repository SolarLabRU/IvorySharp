using System;

namespace Examples.AutoRetry
{
    public interface IWebPageService
    {
        [RetryAspect(RetriesCount = 3, DelayMs = 300)]
        string GetPageContent(string url);
    }

    
    public class WebPageService : IWebPageService
    {
        private static readonly double FailureRate = 0.6;
        private static readonly Random Random = new Random();
        
        public string GetPageContent(string url)
        {
            if (Random.NextDouble() < FailureRate)
                throw new Exception("Network fail");

            return url;
        }
    }
}