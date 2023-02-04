namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Application command option that will be use to resolve argument or create option for that command.
/// </summary>
public sealed class SlashInteractionCommandOptionMetadata : IDiscordAutoCompleteableOption, IEquatable<SlashInteractionCommandOptionMetadata>
{
    private readonly string _name = string.Empty;

    internal SlashInteractionCommandOptionMetadata(SlashInteractionCommandMetadataOptionBuilder builder, IDiscordOption targetOption)
    {
        this.Name = builder.Name;
        this.Description = builder.Description ?? "No Description specified";
        this.OptionType = builder.OptionType;
        this.AutoCompleteCommand = builder.AutoCompleteCommand;
        this.IsRequired = builder.IsRequired;
        this.Range = builder.Range;
        this.ChannelTypes = builder.ChannelTypes;
        this.ChoiceProvider = builder.ChoiceProvider;
        this.TargetOption = targetOption;
        this.Localizations = builder.Localizations;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public string Name { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public string Description { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public bool IsRequired { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public DiscordOptionRange? Range { get; }

    public int Position { get; }

    public ICommandOption TargetOption { get;}

    /// <inheritdoc/>
    [PublicAPI]
    public string? AutoCompleteCommand { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public ApplicationCommandOptionType OptionType { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public IChoiceProvider? ChoiceProvider { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public DiscordLocalizations Localizations { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public IEnumerable<ChannelType>? ChannelTypes { get; }

    public bool Equals(SlashInteractionCommandOptionMetadata? other)
    {
        return other is not null &&
            this.Name == other.Name &&
            this.Description == other.Description &&
            this.IsRequired == other.IsRequired &&
            this.Range == other.Range &&
            this.Position == other.Position &&
            EqualityComparer<ICommandOption>.Default.Equals(this.TargetOption, other.TargetOption) &&
            this.OptionType == other.OptionType &&
            EqualityComparer<IChoiceProvider>.Default.Equals(this.ChoiceProvider, other.ChoiceProvider) &&
            EqualityComparer<IEnumerable<ChannelType>>.Default.Equals(this.ChannelTypes, other.ChannelTypes) &&
            this.Localizations == other.Localizations;
    }

    public override bool Equals(object? obj)
    {
        return obj is SlashInteractionCommandOptionMetadata other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <inheritdoc/>
    [PublicAPI]
    public DiscordApplicationCommandOption ToDiscordOption()
    {
        var choices = this.ChoiceProvider?.GetChoices();

        return new DiscordApplicationCommandOption(this.Name, this.Description, this.OptionType, this.IsRequired, choices, null, this.ChannelTypes, this.AutoCompleteCommand is not null, 
            this.Range?.MinimumValue, this.Range?.MaximumValue, this.Localizations.GetNameLocalizations(), this.Localizations.GetDescriptionLocalizations());
    }

    public static bool operator ==(SlashInteractionCommandOptionMetadata left, SlashInteractionCommandOptionMetadata right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SlashInteractionCommandOptionMetadata left, SlashInteractionCommandOptionMetadata right)
    {
        return !(left == right);
    }
}