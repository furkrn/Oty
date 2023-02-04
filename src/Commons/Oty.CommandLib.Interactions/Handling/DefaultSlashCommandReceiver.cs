namespace Oty.CommandLib.Interactions.Handling;

/// <summary>
/// Default receiver for default slash command entity <see cref="SlashInteractionCommand"/>.
/// </summary>
public sealed class SlashInteractionCommandReceiver : ICommandMetadataReceiver<InteractionCreateEventArgs, SlashInteractionCommand, SlashInteractionContext>
{
    /// <inheritdoc/>
    public bool CanExecute(IReceiverContext<InteractionCreateEventArgs> context)
    {
        return context.EventArgs.Interaction.Type == InteractionType.ApplicationCommand && context.EventArgs.Interaction.Data.Type == ApplicationCommandType.SlashCommand;
    }

    /// <inheritdoc/>
    public Task<IReceiverResult<InteractionCreateEventArgs, SlashInteractionCommand, SlashInteractionContext>?> GetCommandAsync(IReceiverContext<InteractionCreateEventArgs> context)
    {
        var parentCommand = context.Extension.RegisteredCommands
            .OfType<SlashInteractionCommand>()
            .FirstOrDefault(c => ((IDiscordPublishable)c).Id == context.EventArgs.Interaction.Data.Id &&
                c.CommandType == context.EventArgs.Interaction.Data.Type);
        
        var interactionOption = context.EventArgs.Interaction.Data?.Options?.FirstOrDefault();

        var options = context.EventArgs.Interaction.Data?.Options?.SelectMany(o => EnumerableExtensions.Traverse(o, op => op.Options));

        SlashInteractionCommand? command = null;
        if (interactionOption?.Type is (ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup))
        {
            var groupNames = options!.Where(o => o?.Type is (ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup))
                .Select(o => o?.Name);

            command = parentCommand?.GetCommandFromGroup(groupNames);
        }

        command ??= parentCommand;

        if (command is null)
        {
            return Task.FromResult<IReceiverResult<InteractionCreateEventArgs, SlashInteractionCommand, SlashInteractionContext>?>(null);
        }

        var commandContext = new SlashInteractionContext(context.Client, context.Extension, command, context.EventArgs.Interaction, context.EventArgs, context.ServiceProvider);

        var metadataOptions = command.Options.SelectMany(c => c.MetadataOptions)
            .ToArray();

        var interactionOptions = options?.FirstOrDefault(o => o.Type is ApplicationCommandOptionType.SubCommand)?.Options ?? options;

        var receivedMetadataOptions = interactionOptions?
            .Select(o => new { Option = metadataOptions.First(md => md.Name == o.Name), o?.Value })
            ?.ToDictionary(a => a.Option, a => a.Value);

        return Task.FromResult<IReceiverResult<InteractionCreateEventArgs, SlashInteractionCommand, SlashInteractionContext>?>(new ReceiverResult<InteractionCreateEventArgs, SlashInteractionCommand, SlashInteractionContext>(context.EventArgs, commandContext, command, receivedMetadataOptions));
    }
}