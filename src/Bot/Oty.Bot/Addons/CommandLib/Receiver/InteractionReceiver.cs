namespace Oty.Bot.Addons.CommandLib.Receiver;

public sealed class InteractionReceiver : ICommandMetadataReceiver<InteractionCreateEventArgs, AddonMetadata, AddonContext>
{
    public bool CanExecute(IReceiverContext<InteractionCreateEventArgs> context)
    {
        return context.EventArgs.Interaction.Type == InteractionType.ApplicationCommand;
    }

    public Task<IReceiverResult<InteractionCreateEventArgs, AddonMetadata, AddonContext>?> GetCommandAsync(IReceiverContext<InteractionCreateEventArgs> context)
    {
        var command = context.Extension.RegisteredCommands
            .OfType<AddonMetadata>()
            .FirstOrDefault(c => c.Type == (AddonType)context.EventArgs.Interaction.Data.Type);

        if (command is null)
        {
            return Task.FromResult<IReceiverResult<InteractionCreateEventArgs, AddonMetadata, AddonContext>?>(null);
        }

        var receivedValue = new InteractionReceivedValue(context.EventArgs.Interaction);

        var commandContext = new AddonContext(context.Client, context.Extension, command, context.ServiceProvider, receivedValue);

        return Task.FromResult<IReceiverResult<InteractionCreateEventArgs, AddonMetadata, AddonContext>?>
            (new ReceiverResult<InteractionCreateEventArgs, AddonMetadata, AddonContext>(context.EventArgs, commandContext, command));
    }
}