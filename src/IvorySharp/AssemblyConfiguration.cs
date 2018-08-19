using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

[assembly:InternalsVisibleTo("IvorySharp.Integration.CastleWindsor")]
[assembly:InternalsVisibleTo("IvorySharp.Integration.SimpleInjector")]
[assembly:InternalsVisibleTo("IvorySharp.Tests")]
[assembly:InternalsVisibleTo("IvorySharp.Benchmark")]

namespace IvorySharp
{
    /// <summary>
    /// Конфигурационный класс библиотеки.
    /// Вместо AssemblyInfo.
    /// </summary>
    internal sealed class AssemblyConfiguration
    {
    }
}