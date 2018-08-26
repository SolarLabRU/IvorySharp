using System;

namespace Examples.NativeWeaving
{
    public interface IDataService
    {
        [PingAspect] 
        void GenerateData();
    }
    
    public class DataService : IDataService
    {
        public void GenerateData()
        {
            Console.WriteLine("Method GenerateData called");
        }
    }
}