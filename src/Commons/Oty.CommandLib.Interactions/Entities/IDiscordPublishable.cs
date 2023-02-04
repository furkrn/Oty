namespace Oty.CommandLib.Entities;

/// <summary>
/// Defines whether command is required to send to Discord to use it.
/// </summary>
[PublicAPI]
public interface IDiscordPublishable
{
    /// <summary>
    /// Gets the name of the command
    /// </summary>
    [PublicAPI]
    string Name { get; }

    /// <summary>
    /// Gets or sets the unique id of represents the command.
    /// </summary>
    [PublicAPI]
    ulong Id { get; set; }

    /// <summary>
    /// Gets the registered id of the guild. <see langword="null"/> when registered globally.
    /// </summary>
    ulong? RegisteredGuildId { get; }

    /// <summary>
    /// Gets whether the application command enabled by default.
    /// <para><b>This value is only present when command isn't contained on group.</b></para>
    /// </summary>
    [PublicAPI]
    bool DefaultPermission { get; }

    /// <summary>
    /// Indicates the specified application command is avaliable for private channels of the application.
    /// </summary>
    [PublicAPI]
    bool IsPrivateChannelsAllowed { get; }

    /// <summary>
    /// Gets the Type of the application command.
    /// </summary>
    [PublicAPI]
    ApplicationCommandType CommandType { get; }

    /// <summary>
    /// Gets the required permission to run this command on a specified guild.
    /// </summary>
    [PublicAPI]
    Permissions CommandPermissions { get; }

    /// <summary>
    /// Gets the localizations for this command.
    /// </summary>
    [PublicAPI]
    DiscordLocalizations? Localizations { get; }

    /// <summary>
    /// Converts the command to an instance of <see cref="DiscordApplicationCommand"/>
    /// </summary>
    /// <returns>An instance of <see cref="DiscordApplicationCommand"/> from the command.</returns>
    [PublicAPI]
    DiscordApplicationCommand ToDiscordCommand();

    /// <summary>
    /// Creates new instance of the command for another guild.
    /// </summary>
    /// <param name="guildId">Guild to clone</param>
    /// <returns>An cloned instance of <see cref="IDiscordPublishable"/></returns>
    IDiscordPublishable CloneToGuild(ulong? guildId);
}