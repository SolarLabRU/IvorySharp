using System;
using IvorySharp.Aspects.Weaving;

namespace Examples.NativeWeaving
{
    class Program
    {
        static void Main(string[] args)
        {
            var weaver = AspectWeaverFactory.Create();
            var service = (IDataService)weaver.Weave(new DataService(), typeof(IDataService), typeof(DataService));
            
            service.GenerateData();
        }
    }
}