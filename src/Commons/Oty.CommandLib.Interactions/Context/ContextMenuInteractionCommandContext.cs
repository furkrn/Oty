namespace Oty.CommandLib.Interactions;

/// <summary>
/// Represents context for context menu interactions.
/// </summary>
[PublicAPI]
public sealed class ContextMenuInteractionCommandContext : BaseInteractionCommandContext, IEventContext<ContextMenuInteractionCreateEventArgs>
{
    internal ContextMenuInteractionCommandContext(DiscordClient client, IOtyCommandsExtension sender, ContextMenuInteractionCommand commandMetadata, DiscordInteraction interaction, ContextMenuInteractionCreateEventArgs eventArgs, IServiceProvider? serviceProvider) : base(client, sender, commandMetadata, interaction, serviceProvider)
    {
        this.Event = eventArgs;
    }

    /// <summary>
    /// Gets the message that is invoked this interaction if it's valiable.
    /// </summary>
    [PublicAPI]
    public DiscordMessage? TargetMessage => this.Event.TargetMessage;

    [PublicAPI]
    public DiscordUser? TargetUser => this.Event.TargetUser;

    public ContextMenuInteractionCreateEventArgs Event { get; }
}