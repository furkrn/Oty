namespace Oty.CommandLib.Interactions.Entities;

public interface IDiscordMetadataOption : IMetadataOption
{
    /// <summary>
    /// Gets the specified discord option type of the option.
    /// </summary>
    [PublicAPI]
    ApplicationCommandOptionType OptionType { get; }

    /// <summary>
    /// Gets the specified choice provider of the option.
    /// </summary>
    [PublicAPI]
    IChoiceProvider? ChoiceProvider { get; }

    /// <summary>
    /// Gets the limited channel types of the option.
    /// <para>Valiable if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Channel"/></para>
    /// </summary>
    [PublicAPI]
    IEnumerable<ChannelType>? ChannelTypes { get; }

    /// <summary>
    /// Gets the description of the option.
    /// </summary>
    [PublicAPI]
    string? Description { get; }

    /// <summary>
    /// Gets the both minimum and maximum values of the option.
    /// <para>Valiable if <see cref="OptionType"/> is <see cref="ApplicationCommandOptionType.Number"/> or <see cref="ApplicationCommandOptionType.Integer"/></para>
    /// </summary>
    [PublicAPI]
    DiscordOptionRange? Range { get; }

    /// <summary>
    /// Gets whether option is it's autocompletable.
    /// </summary>
    [PublicAPI]
    bool IsAutoComplete { get; }

    /// <summary>
    /// Gets whether option is required or not.
    /// </summary>
    [PublicAPI]
    bool IsRequired { get; }

    [PublicAPI]
    DiscordLocalizations Localizations { get; }

    [PublicAPI]
    DiscordApplicationCommandOption ToDiscordOption();
}