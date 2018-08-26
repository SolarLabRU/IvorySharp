using System;

namespace Examples.AspectSelectionFromClass
{
    public interface IDataService
    {
        void GenerateData();
    }
    
    public class DataService : IDataService
    {
        [PingAspect] // <- Аспект на методе класса, который объявлен в интерфейсе
        public void GenerateData()
        {
            Console.WriteLine("Method GenerateData called");
        }
    }
}