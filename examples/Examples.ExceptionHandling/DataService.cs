using System;

namespace Examples.ExceptionHandling
{
    [SwallowExceptionAspect(Order = 1)]
    public interface IDataService
    {
        [ReplaceExceptionAspect(Order = 0)]
        void GenerateData();
    }
    
    public class DataService : IDataService
    {
        public void GenerateData()
        {
            throw new Exception("Generate data throwed");
        }
    }
}