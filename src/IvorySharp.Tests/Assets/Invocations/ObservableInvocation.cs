using System;

namespace IvorySharp.Tests.Assets.Invocations
{
    public class ObservableInvocation : BypassInvocation
    {
        public bool IsProceedCalled { get; private set; }
        
        public ObservableInvocation(Type declaringType, object instance, string methodName) 
            : base(declaringType, instance, methodName)
        {
        }

        public ObservableInvocation(Type declaringType, object instance, string methodName, object[] arguments) 
            : base(declaringType, instance, methodName, arguments)
        {
        }

        public override object Proceed()
        {
            IsProceedCalled = true;
            return base.Proceed();
        }
    }
}