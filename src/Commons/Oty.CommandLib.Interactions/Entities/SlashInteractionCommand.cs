namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Provides invokation of the "/" input based interactions.
/// </summary>
public sealed class SlashInteractionCommand : BaseInteractionCommandMetadata, ICommandArgumentsAvaliable<SlashInteractionCommandOption>, IGroupableMetadata<SlashInteractionCommand>
{
    internal SlashInteractionCommand(InteractionCommandBuilder builder, Type moduleType) : base(builder, moduleType)
    {
        this.Options = builder.Options;
        this.Groups = builder.Subcommands;
        this.Description = builder.Description ?? "No Description Specified";
    }

    /// <inheritdoc/>
    [PublicAPI]
    public IReadOnlyList<SlashInteractionCommandOption> Options { get; internal init; }
    
    /// <inheritdoc/>
    [PublicAPI]
    public IReadOnlyList<IGroup<SlashInteractionCommand>>? Groups { get; internal init; }

    /// <summary>
    /// Gets the description of the application command.
    /// </summary>
    [PublicAPI]
    public string Description { get; internal init; }

    /// <inheritdoc/>
    public override DiscordApplicationCommand ToDiscordCommand()
    {
        var discordOptions = this.Options?.Count > 0
            ? this.Options.SelectMany(o => o.CreateOptions())
            : this.Groups?.ConvertToDiscordOptions();

        return new DiscordApplicationCommand(this.Name, this.Description, discordOptions, this.DefaultPermission,
            this.CommandType, this.Localizations.GetNameLocalizations(), this.Localizations.GetDescriptionLocalizations(), this.IsPrivateChannelsAllowed, this.CommandPermissions);
    }

    /// <inheritdoc/>
    public override bool Equals(BaseCommandMetadata? other)
    {
        return other is SlashInteractionCommand metadata && 
            base.Equals(metadata) &&
            this.Options == metadata.Options &&
            this.Groups == metadata.Groups;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.DefaultPermission, this.Name, this.Description, this.Options, this.Localizations, this.Groups,
            this.IsPrivateChannelsAllowed, this.ModuleType);
    }
}