namespace Oty.CommandLib;

/// <summary>
/// Provides configuration for the application command extension.
/// </summary>
[PublicAPI]
public sealed class OtyCommandsConfiguration
{
    /// <summary>
    /// Sets the service provider for commands.
    /// </summary>
    [PublicAPI]
    public IServiceProvider? RegisteredServices { get; init; }

    /// <summary>
    /// Sets whether implementention of the parser handler to be used, <see langword="null"/> to use default implementention.
    /// </summary>
    [PublicAPI]
    public ICommandParser? CommandParser { get; init; }

    /// <summary>
    /// Sets the executor of the commands.
    /// </summary>
    [PublicAPI]
    public ICommandExecutor? Executor { get; init; }
}