using System;

namespace IvorySharp.Tests.Services
{
    public class MultipleBoundaryAspectsService : IMultipleBoundaryAspectsService
    {
        public int Identity(int argument)
        {
            return argument;
        }

        public int IdentityThrowOnEntry(int argument)
        {
            return argument;
        }

        public int IdentityThrowOnSuccess(int argument)
        {
            return argument;
        }

        public int IdentityThrowOnExit(int argument)
        {
            return argument;
        }

        public int IdentityInnerSuccess(int argument)
        {
            throw new ArgumentException();
        }

        public int IdentityReturnOnEntry(int argument)
        {
            return argument;
        }

        public int IdentityThrowPipelineOnEntry(int argument)
        {
            return argument;
        }
    }
}