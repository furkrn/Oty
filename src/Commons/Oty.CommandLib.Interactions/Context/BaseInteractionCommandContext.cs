namespace Oty.CommandLib.Interactions;

/// <summary>
/// Represents context for Discord interaction based commands.
/// </summary>
[PublicAPI]
public abstract class BaseInteractionCommandContext : BaseCommandContext
{
    /// <summary>
    /// Creates an instance of <see cref="BaseInteractionCommandContext"/> from received context from handler.
    /// </summary>
    /// <returns></returns>
    internal BaseInteractionCommandContext(DiscordClient client, IOtyCommandsExtension sender, BaseCommandMetadata metadata, DiscordInteraction interaction, IServiceProvider? serviceProvider) : base(client, sender, metadata, serviceProvider)
    {
        this.Interaction = interaction;
    }

    /// <summary>
    /// Gets the interaction that is revoked.
    /// </summary>
    [PublicAPI]
    public DiscordInteraction Interaction { get; protected internal init; }

    /// <summary>
    /// Gets the user that executed the interaction.
    /// </summary>
    [PublicAPI]
    public DiscordUser User => this.Interaction.User;

    /// <summary>
    /// Gets the guild member that executed the interaction. Only valiable if interaction executed on a guild.
    /// </summary>
    [PublicAPI]
    public DiscordMember? Member => this.Interaction.User as DiscordMember;

    /// <summary>
    /// Gets the channel where interaction being executed.
    /// </summary>
    [PublicAPI]
    public DiscordChannel Channel => this.Interaction.Channel;

    /// <summary>
    /// Gets the guild where interaction being executed.
    /// </summary>
    [PublicAPI]
    public DiscordGuild Guild => this.Interaction.Guild;

    /// <summary>
    /// Gets the id of the guild where interaction being executed.
    /// </summary>
    [PublicAPI]
    public ulong? GuildId => this.Interaction.GuildId;

    /// <summary>
    /// Gets the id of the application command.
    /// </summary>
    [PublicAPI]
    public ulong CommandId => this.Interaction.Data.Id;

    /// <summary>
    /// Gets the type of the application command.
    /// </summary>
    [PublicAPI]
    public ApplicationCommandType ApplicationCommandType => this.Interaction.Data.Type;

    /// <inheritdoc cref="DiscordInteraction.CreateResponseAsync(InteractionResponseType, DiscordInteractionResponseBuilder)"/>
    [PublicAPI]
    public Task CreateResponseAsync(InteractionResponseType type, DiscordInteractionResponseBuilder? builder = null)
    {
        return this.Interaction.CreateResponseAsync(type, builder);
    }

    /// <summary>
    /// Creates a response to this interaction.
    /// </summary>
    /// <param name="responseBuilder">The data to send.</param>
    [PublicAPI]
    public Task CreateResponseAsync(DiscordInteractionResponseBuilder responseBuilder)
    {
        return this.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
    }

    /// <inheritdoc cref="DiscordInteraction.CreateFollowupMessageAsync(DiscordFollowupMessageBuilder)"/>
    [PublicAPI]
    public Task CreateFollowupMessageAsync(DiscordFollowupMessageBuilder builder)
    {
        return this.Interaction.CreateFollowupMessageAsync(builder);
    }

    /// <inheritdoc cref="DiscordInteraction.EditOriginalResponseAsync(DiscordWebhookBuilder, IEnumerable{DiscordAttachment})"/>
    [PublicAPI]
    public Task<DiscordMessage> EditResponseAsync(DiscordWebhookBuilder builder, IEnumerable<DiscordAttachment>? attachments = null)
    {
        return this.Interaction.EditOriginalResponseAsync(builder, attachments);
    }

    /// <inheritdoc cref="DiscordInteraction.EditFollowupMessageAsync(ulong, DiscordWebhookBuilder, IEnumerable{DiscordAttachment})"/>
    [PublicAPI]
    public Task<DiscordMessage> EditFollowupMessageAsync(ulong id, DiscordWebhookBuilder builder, IEnumerable<DiscordAttachment>? attachments = null)
    {
        return this.Interaction.EditFollowupMessageAsync(id, builder, attachments);
    }

    [PublicAPI] // TODO : Will document this later...
    public Task DeferAsync(bool ephemeral = false)
    {
        return this.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(ephemeral));
    }

    /// <inheritdoc cref="DiscordInteraction.DeleteFollowupMessageAsync(ulong)"/>
    [PublicAPI]
    public Task DeleteFollowupMessageAsync(ulong messageId)
    {
        return this.Interaction.DeleteFollowupMessageAsync(messageId);
    }

    /// <inheritdoc cref="DiscordInteraction.DeleteOriginalResponseAsync()"/>
    [PublicAPI]
    public Task DeleteOriginalResponseAsync()
    {
        return this.Interaction.DeleteOriginalResponseAsync();
    }

    /// <inheritdoc cref="DiscordInteraction.DeleteOriginalResponseAsync()"/>
    [PublicAPI]
    public Task<DiscordMessage> GetOriginalResponseAsync()
    {
        return this.Interaction.GetOriginalResponseAsync();
    }

    /// <inheritdoc cref="DiscordInteraction.GetFollowupMessageAsync(ulong)"/>
    [PublicAPI]
    public Task<DiscordMessage> GetFollowupMessageAsync(ulong messageId)
    {
        return this.Interaction.GetFollowupMessageAsync(messageId);
    }
}