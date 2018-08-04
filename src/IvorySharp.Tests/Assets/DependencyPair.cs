using System;

namespace IvorySharp.Tests.Assets
{
    public class DependencyPair
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }

        public DependencyPair(Type service, Type implementation)
        {
            ServiceType = service;
            ImplementationType = implementation;
        }
    }
}