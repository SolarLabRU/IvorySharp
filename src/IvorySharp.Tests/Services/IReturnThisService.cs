using IvorySharp.Tests.Aspects;

namespace IvorySharp.Tests.Services
{
    public interface IReturnThisService
    {
        [BypassAspect]
        IReturnThisService ReturnSelf();
    }

    public class ReturnThisService : IReturnThisService
    {
        public IReturnThisService ReturnSelf()
        {
            return this;
        }
    }
}