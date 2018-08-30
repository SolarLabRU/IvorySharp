using System.Linq;

namespace IvorySharp.Tests.Utility
{
    public static class Args
    {
        public static T[] Pack<T>(params T[] args)
        {
            return args;
        }

        public static object[] Box<T>(params T[] args)
        {
            return args.Select(a => (object)a).ToArray();
        }
    }
}