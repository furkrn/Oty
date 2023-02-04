namespace Oty.CommandLib.Interactions.Handling;

/// <summary>
/// A generic handler for receiver <see cref="IDiscordPublishable"/> commands from <typeparamref name="TInteractionEventArgs"/>.
/// </summary>
/// <typeparam name="TInteractionEventArgs">Type of the event to find command.</typeparam>
/// <typeparam name="TPublishableCommand">Type of the publishable command to find..</typeparam>
[PublicAPI]
public class PublishableCommandReceiver<TInteractionEventArgs, TPublishableCommand, TContext> : ICommandMetadataReceiver<TInteractionEventArgs, TPublishableCommand, TContext>
    where TInteractionEventArgs : InteractionCreateEventArgs
    where TPublishableCommand : BaseCommandMetadata, IDiscordPublishable
    where TContext : BaseCommandContext
{
    private readonly Func<TPublishableCommand, IReceiverContext<TInteractionEventArgs>, TContext> _funcContext;

    public PublishableCommandReceiver(Func<TPublishableCommand, IReceiverContext<TInteractionEventArgs>, TContext> funcContext)
    {
        this._funcContext = funcContext ?? throw new ArgumentNullException(nameof(funcContext));
    }

    /// <inheritdoc/>
    [PublicAPI]
    public virtual bool CanExecute(IReceiverContext<TInteractionEventArgs> context)
    {
        return context.EventArgs.Interaction.Type == InteractionType.ApplicationCommand;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public virtual Task<IReceiverResult<TInteractionEventArgs, TPublishableCommand, TContext>?> GetCommandAsync(IReceiverContext<TInteractionEventArgs> context)
    {
        var command = context.Extension.RegisteredCommands
            .OfType<TPublishableCommand>()
            .FirstOrDefault(c => c.Id == context.EventArgs.Interaction.Data.Id
                && c.CommandType == context.EventArgs.Interaction.Data.Type);

        if (command is null)
        {
            return Task.FromResult<IReceiverResult<TInteractionEventArgs, TPublishableCommand, TContext>?>(null);
        }

        var commandContext = this._funcContext(command, context);

        return Task.FromResult<IReceiverResult<TInteractionEventArgs, TPublishableCommand, TContext>?>(new ReceiverResult<TInteractionEventArgs, TPublishableCommand, TContext>(context.EventArgs, commandContext, command, null));
    }
}