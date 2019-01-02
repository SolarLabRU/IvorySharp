using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

[assembly:InternalsVisibleTo("IvorySharp.Integration.CastleWindsor")]
[assembly:InternalsVisibleTo("IvorySharp.Integration.SimpleInjector")]
[assembly:InternalsVisibleTo("IvorySharp.Integration.Microsoft.DependencyInjection")]
[assembly:InternalsVisibleTo("IvorySharp.Tests")]
[assembly:InternalsVisibleTo("IvorySharp.Benchmark")]

namespace IvorySharp
{
    /// <summary>
    /// Assembly configuration class for internal purpose (kind of AssemblyInfo.cs).
    /// </summary>
    internal sealed class AssemblyConfiguration
    {
    }
}