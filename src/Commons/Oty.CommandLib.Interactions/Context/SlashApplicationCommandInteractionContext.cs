namespace Oty.CommandLib.Interactions;

/// <summary>
/// Represents context for slash interactions.
/// </summary>
[PublicAPI]
public sealed class SlashInteractionContext : BaseInteractionCommandContext, IEventContext<InteractionCreateEventArgs>
{
    internal SlashInteractionContext(DiscordClient client, IOtyCommandsExtension sender, SlashInteractionCommand commandMetadata, DiscordInteraction interaction, InteractionCreateEventArgs eventArgs, IServiceProvider? serviceProvider) : base(client, sender, commandMetadata, interaction, serviceProvider)
    {
        this.Event = eventArgs;
    }

    public InteractionCreateEventArgs Event { get;}
}