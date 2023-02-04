namespace Oty.CommandLib.EventArgs;

/// <summary>
/// Represents arguments for event <see cref="OtyCommandsExtension.CommandHandled"/>.
/// </summary>
[PublicAPI]
public sealed class CommandHandledEventArgs : AsyncEventArgs
{
    internal CommandHandledEventArgs(DiscordClient client, CommandResult commandResult)
    {
        this.Client = client;
        this.ExecutionResult = commandResult;
    }

    /// <summary>
    /// Gets the Discord client.
    /// </summary>
    [PublicAPI]
    public DiscordClient Client { get; }

    [PublicAPI]
    public CommandResult ExecutionResult { get; }
}