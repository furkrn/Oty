namespace Oty.CommandLib.Handling;

/// <summary>
/// Defines the context that is used by receiver and handler.
/// </summary>
/// <typeparam name="TEvent">Type of the event to receive.</typeparam>
[PublicAPI]
public interface IReceiverContext<out TEvent>
    where TEvent : DiscordEventArgs
{
    /// <summary>
    /// Gets the received event's arguments.
    /// </summary>
    [PublicAPI]
    TEvent EventArgs { get; }

    /// <summary>
    /// Gets the Discord client.
    /// </summary>
    [PublicAPI]
    DiscordClient Client { get; }

    /// <summary>
    /// Gets the service provider of the extension.
    /// </summary>
    [PublicAPI]
    IServiceProvider? ServiceProvider { get; }

    /// <summary>
    /// Gets the extension.
    /// </summary>
    [PublicAPI]
    IOtyCommandsExtension Extension { get; }
}
