namespace Oty.CommandLib.Interactions.Entities;

public abstract class BaseInteractionCommandMetadata : BaseCommandMetadata, IDiscordPublishable
{
    internal BaseInteractionCommandMetadata(InteractionCommandBuilder builder, Type commandType) : base(builder.CommandName, commandType)
    {
        this.DefaultPermission = builder.DefaultPermission;
        this.IsPrivateChannelsAllowed = builder.IsPrivateChannelsAllowed;
        this.CommandType = builder.CommandType;
        this.CommandPermissions = builder.CommandPermissions;
        this.RegisteredGuildId = builder.GuildId;
        this.Localizations = builder.Localizations;
    }

    [PublicAPI]
    ulong IDiscordPublishable.Id { get; set; }

    /// </inheritdoc>
    [PublicAPI]
    public ulong? RegisteredGuildId { get; protected set; }

    /// </inheritdoc>
    [PublicAPI]
    public bool DefaultPermission { get; protected set; }

    /// </inheritdoc>
    [PublicAPI]
    public bool IsPrivateChannelsAllowed { get; protected set; }

    /// </inheritdoc>
    [PublicAPI]
    public ApplicationCommandType CommandType { get; protected set; }

    /// </inheritdoc>
    [PublicAPI]
    public Permissions CommandPermissions { get; protected set; }

    /// </inheritdoc>
    public DiscordLocalizations Localizations { get; protected set; }

    /// </inheritdoc>
    [PublicAPI]
    public abstract DiscordApplicationCommand ToDiscordCommand();

    public override bool Equals(BaseCommandMetadata? other)
    {
        return base.Equals(other) &&
            other is BaseInteractionCommandMetadata metadata &&
            this.RegisteredGuildId == metadata.RegisteredGuildId &&
            this.DefaultPermission == metadata.DefaultPermission &&
            this.IsPrivateChannelsAllowed == metadata.IsPrivateChannelsAllowed &&
            this.CommandType == metadata.CommandType &&
            this.CommandPermissions == metadata.CommandPermissions &&
            this.Localizations == metadata.Localizations;
    }

    /// </inheritdoc>
    [PublicAPI]
    public virtual IDiscordPublishable CloneToGuild(ulong? guildId)
    {
        var instance = (BaseInteractionCommandMetadata)this.MemberwiseClone();
        instance.RegisteredGuildId = guildId;
        ((IDiscordPublishable)instance).Id = 0;
        return instance;
    }
}