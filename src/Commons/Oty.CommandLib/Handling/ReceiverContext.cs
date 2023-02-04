namespace Oty.CommandLib.Handling;

internal sealed class ReceiverContext<TEvent> : IReceiverContext<TEvent>
    where TEvent : DiscordEventArgs
{
    internal ReceiverContext(TEvent eventArgs, DiscordClient client, IOtyCommandsExtension extension)
    {
        this.EventArgs = eventArgs;
        this.Client = client;
        this.Extension = extension;
    }

    /// <summary>
    /// Gets the received event's arguments.
    /// </summary>
    public TEvent EventArgs { get; }

    /// <summary>
    /// Gets the Discord client.
    /// </summary>
    public DiscordClient Client { get; }

    /// <summary>
    /// Gets the service provider of the extension.
    /// </summary>
    public IServiceProvider? ServiceProvider { get; internal init; }

    /// <summary>
    /// Gets the extension.
    /// </summary>
    public IOtyCommandsExtension Extension { get; }
}