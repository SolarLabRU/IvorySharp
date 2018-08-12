namespace IvorySharp.Proxying
{
    /// <summary>
    /// Позициии параметров в упакованном массиве <see cref="PackedArguments"/>.
    /// </summary>
    internal enum PackedArgPosition
    {
        Proxy = 0,
        DeclaringType = 1,
        MethodTokenKey = 2,
        MethodArguments = 3,
        GenericArgs = 4,
        ReturnValue = 5
    }
}