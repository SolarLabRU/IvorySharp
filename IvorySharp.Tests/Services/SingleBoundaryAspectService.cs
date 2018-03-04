using System;

namespace IvorySharp.Tests.Services
{
    public class SingleBoundaryAspectService : ISingleBoundaryAspectService
    {
        public void BypassEmptyMethod()
        {
        }

        public int Identity(int argument)
        {
            return argument;
        }

        public int Identity2(int argument)
        {
            return argument;
        }

        public int Identity3(int argument)
        {
            return argument;
        }

        public void ExceptionalEmptyMethod()
        {
            throw new Exception();
        }

        public void ExceptionalEmptyMethod2()
        {
            throw new Exception();
        }

        public void ExceptionalEmptyMethod3()
        {
            throw new Exception();
        }

        public int ExceptionalIdentity(int argument)
        {
            throw new Exception();
        }

        public int ExceptionalIdentity2(int argument)
        {
            throw new Exception();
        }

        public object ExceptionalRef()
        {
            throw new Exception();
        }

        public object ExceptionalRef2()
        {
            throw new Exception();
        }
    }
}