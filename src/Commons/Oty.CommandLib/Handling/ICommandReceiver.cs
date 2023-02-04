namespace Oty.CommandLib.Handling;

/// <summary>
/// Defines the receiver that is known to find command from extension with target event.
/// </summary>
[PublicAPI]
public interface ICommandMetadataReceiver
{
}

/// <inheritdoc/>
/// <typeparam name="TEvent">Type of the event to receive.</typeparam>
/// <typeparam name="TCommand">Type of the command to find.</typeparam>
/// <typeparam name="TContext">Type of the context of the command.</typeparam>
[PublicAPI]
public interface ICommandMetadataReceiver<TEvent, TCommand, TContext> : ICommandMetadataReceiver
    where TEvent : DiscordEventArgs
    where TCommand : BaseCommandMetadata
    where TContext : BaseCommandContext
{
    /// <summary>
    /// Gets wheter this receiver can be executed.
    /// </summary>
    /// <param name="context">Context received from the client.</param>
    /// <returns></returns>
    [PublicAPI]
    bool CanExecute(IReceiverContext<TEvent> context);

    /// <summary>
    /// Finds the command from <see cref="TEvent"/>
    /// </summary>
    /// <param name="context">Context received from the client.</param>
    /// <returns>A wrapper that may contain <typeparamref name="TCommand"/> or not.</returns>
    [PublicAPI]
    Task<IReceiverResult<TEvent, TCommand, TContext>?> GetCommandAsync(IReceiverContext<TEvent> context);
}