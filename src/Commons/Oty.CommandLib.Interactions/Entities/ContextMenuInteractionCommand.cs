namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Provides invokation of UI-Based Interactions.
/// </summary>
public sealed class ContextMenuInteractionCommand : BaseInteractionCommandMetadata
{
    internal ContextMenuInteractionCommand(InteractionCommandBuilder builder, Type moduleType) : base(builder, moduleType)
    {
    }

    /// <inheritdoc/>
    public override DiscordApplicationCommand ToDiscordCommand()
    {
        return new DiscordApplicationCommand(this.Name, string.Empty, null, this.DefaultPermission, this.CommandType, 
            this.Localizations.GetNameLocalizations(), this.Localizations.GetDescriptionLocalizations(), this.IsPrivateChannelsAllowed, this.CommandPermissions);
    }

    /// <inheritdoc/>
    public override bool Equals(BaseCommandMetadata? other)
    {
        return other is ContextMenuInteractionCommand && base.Equals(other);
    }

    // resharper disable NonReadonlyMemberInGetHashCode

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name, this.CommandPermissions, this.RegisteredGuildId, this.Localizations, this.IsPrivateChannelsAllowed);
    }

    // resharper restore all
}