using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IvoryProxy
{
    internal class IvoryInternal
    {
        public static IReadOnlyCollection<MethodBase> NotInterceptableMethods { get; }
            = new List<MethodBase>
            {
                typeof(object).GetMethod($"{nameof(object.GetType)}")
            };
    }
}
