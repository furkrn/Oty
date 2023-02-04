namespace Oty.CommandLib.Entities;

/// <summary>
/// Represents the values received from an receiver.
/// </summary>
/// <typeparam name="TEventArgs">Event Type</typeparam>
/// <typeparam name="TCommand">Command Type</typeparam>
/// <typeparam name="TContext">Context Type</typeparam>
[PublicAPI]
public interface IReceiverResult<out TEventArgs, out TCommand, out TContext>
    where TContext : BaseCommandContext
    where TCommand : BaseCommandMetadata
    where TEventArgs : DiscordEventArgs
{
    /// <summary>
    /// Gets the context.
    /// </summary>
    [PublicAPI]
    TContext CommandContext { get; }

    /// <summary>
    /// Gets the received metadata.
    /// </summary>
    [PublicAPI]
    TCommand CommandMetadata { get; }

    /// <summary>
    /// Gets the received event.
    /// </summary>
    [PublicAPI]
    TEventArgs EventArgs { get; }

    /// <summary>
    /// Gets the received options for command.
    /// </summary>
    [PublicAPI]
    IReadOnlyDictionary<IMetadataOption, object?>? MetadataOptions { get; }
}