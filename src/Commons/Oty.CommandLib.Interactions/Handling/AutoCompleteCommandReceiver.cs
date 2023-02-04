namespace Oty.CommandLib.Interactions.Handling;

/// <summary>
/// Receiver of the autocomplete command.
/// </summary>
public sealed class AutoCompleteCommandReceiver<TTargetCommand, TTargetContext> : ICommandMetadataReceiver<InteractionCreateEventArgs, AutoCompleteInteractionCommand, AutoCompleteInteractionContext>
    where TTargetCommand : BaseCommandMetadata, IDiscordPublishable, ICommandArgumentsAvaliable<IDiscordOption>
    where TTargetContext : BaseInteractionCommandContext
{
    private readonly ICommandMetadataReceiver<InteractionCreateEventArgs, TTargetCommand, TTargetContext> _targetCommandReceiver;

    /// <summary>
    /// Creates an instance of <see cref="AutoCompleteReceiver{TAutoCompleteCommand, TTargetCommand}"/>.
    /// </summary>
    /// <param name="targetCommandReceiver">Receiver of the command that contains <typeparamref name="TAutoCompleteCommand"/> autocompletable options.</param>
    /// <exception cref="ArgumentNullException"><paramref name="targetCommandReceiver"/> is null.</exception>
    public AutoCompleteCommandReceiver(ICommandMetadataReceiver<InteractionCreateEventArgs, TTargetCommand, TTargetContext> targetCommandReceiver)
    {
        this._targetCommandReceiver = targetCommandReceiver ?? throw new ArgumentNullException(nameof(targetCommandReceiver));
    }

    /// <inheritdoc/>
    public bool CanExecute(IReceiverContext<InteractionCreateEventArgs> context)
    {
        return context.EventArgs.Interaction.Type == InteractionType.AutoComplete;
    }

    /// <inheritdoc/>
    public async Task<IReceiverResult<InteractionCreateEventArgs, AutoCompleteInteractionCommand, AutoCompleteInteractionContext>?> GetCommandAsync(IReceiverContext<InteractionCreateEventArgs> context)
    {
        var targetCommandReceiverContext = new ReceiverContext(context);

        var receivedCommand = await this._targetCommandReceiver.GetCommandAsync(targetCommandReceiverContext).ConfigureAwait(false);

        if (receivedCommand is null)
        {
            return null;
        }

        var focusedInteractionOption = context.EventArgs.Interaction.Data.Options.SelectMany(o => EnumerableExtensions.Traverse(o, o => o!.Options))
            .SingleOrDefault(o => o!.Focused);

        var autoCompleteCommandName = (receivedCommand.CommandMetadata.Options?
            .SelectMany(c => c.MetadataOptions)
            .FirstOrDefault(o => o.Name == focusedInteractionOption?.Name) as IDiscordAutoCompleteableOption)?
            .AutoCompleteCommand;

        var autoCompleteCommand = context.Extension.RegisteredCommands
            .OfType<AutoCompleteInteractionCommand>()
            .FirstOrDefault(ac => ac.Name == autoCompleteCommandName);

        if (autoCompleteCommand is null)
        {
            return null;
        }

        var commandContext = new AutoCompleteInteractionContext(context.Client, context.Extension, autoCompleteCommand, context.EventArgs.Interaction, context.ServiceProvider);

        return new ReceiverResult<InteractionCreateEventArgs, AutoCompleteInteractionCommand, AutoCompleteInteractionContext>(context.EventArgs, commandContext, autoCompleteCommand);
    }

    private sealed class ReceiverContext : IReceiverContext<InteractionCreateEventArgs>
    {
        public ReceiverContext(IReceiverContext<InteractionCreateEventArgs> context)
        {
            this.EventArgs = context.EventArgs;
            this.Client = context.Client;
            this.ServiceProvider = context.ServiceProvider;
            this.Extension = context.Extension;
        }

        public InteractionCreateEventArgs EventArgs { get; }

        public DiscordClient Client { get; }

        public IServiceProvider? ServiceProvider { get; }

        public IOtyCommandsExtension Extension { get; }
    }
}