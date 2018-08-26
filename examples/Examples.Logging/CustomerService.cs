namespace Examples.Logging
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public interface ICustomerService
    {
        [LogMethodArgumentsAspect]
        void SaveCustomer(Customer customer);
    }
    
    public class CustomerService : ICustomerService
    {
        public void SaveCustomer(Customer customer)
        {
            // do nothing
        }
    }
}