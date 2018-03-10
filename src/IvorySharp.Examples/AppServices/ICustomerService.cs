using IvorySharp.Examples.Aspects;
using IvorySharp.Examples.Models;

namespace IvorySharp.Examples.AppServices
{
    [TraceArgumentsAspect]
    public interface ICustomerService
    {
        [TransactionAspect]
        void CreateCustomer(Customer customer);

        [CacheAspect]
        Customer GetCustomer(int id);
    }
}