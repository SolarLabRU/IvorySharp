using IvorySharp.Aspects.Weaving;

namespace Examples.NativeWeaving
{
    class Program
    {
        static void Main(string[] args)
        {
            var weaver = AspectWeaverFactory.Create();
            var service = weaver.Weave<IDataService, DataService>(new DataService());
            
            service.GenerateData();
        }
    }
}