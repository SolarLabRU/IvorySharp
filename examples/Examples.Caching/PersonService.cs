using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Examples.Caching
{
    public interface IPersonService
    {
        [CacheAspect]
        Person GetPerson(int id);
    }
    
    public class PersonService : IPersonService
    {
        private readonly Person[] _persons =
        {
            new Person{ Id = 0, Name = "John", FullName = "Doe" },
            new Person{ Id = 1, Name = "Karl", FullName = "Cook "}
        };
        
        public Person GetPerson(int id)
        {
            Console.WriteLine($"PersonService: Searching person with Id = '{id}'");
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            return _persons.FirstOrDefault(p => p.Id == id);
        }
    }
}