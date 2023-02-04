namespace Oty.CommandLib.Handling;

/// <inheritdoc cref="IReceiverResult{TEvent, TCommand, TContext}"/>
[PublicAPI]
public sealed class ReceiverResult<TEvent, TCommand, TContext> : IReceiverResult<TEvent, TCommand, TContext>
    where TEvent : DiscordEventArgs
    where TCommand : BaseCommandMetadata
    where TContext : BaseCommandContext
{
    /// <summary>
    /// Creates an instance of <see cref="ReceiverResult{TEvent, TCommand, TContext}"/>
    /// </summary>
    /// <param name="eventArgs">The received event.</param>
    /// <param name="context">The context of the command.</param>
    /// <param name="command">The command.</param>
    /// <param name="options">The options of the command.</param>
    public ReceiverResult(TEvent eventArgs, TContext context, TCommand command, IReadOnlyDictionary<IMetadataOption, object?>? options = null)
    {
        this.CommandContext = context ?? throw new ArgumentNullException(nameof(context));
        this.EventArgs = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs));
        this.CommandMetadata = command ?? throw new ArgumentNullException(nameof(command));
        this.MetadataOptions = options;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public TContext CommandContext { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public TCommand CommandMetadata { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public TEvent EventArgs { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public IReadOnlyDictionary<IMetadataOption, object?>? MetadataOptions { get; }
}