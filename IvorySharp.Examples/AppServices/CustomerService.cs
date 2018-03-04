using System.Collections.Generic;
using System.Linq;
using IvorySharp.Examples.Models;

namespace IvorySharp.Examples.AppServices
{
    public class CustomerService : ICustomerService
    {
        private List<Customer> _customers;

        public CustomerService()
        {
            _customers = new List<Customer>();
        }
        
        public void CreateCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public Customer GetCustomer(int id)
        {
            return _customers.FirstOrDefault(c => c.Id == id);
        }
    }
}