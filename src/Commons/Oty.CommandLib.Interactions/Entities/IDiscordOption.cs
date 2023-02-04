namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Represents a discord interaction command compliant command options for any <see cref="BaseCommandMetadata"/> with <see cref="IDiscordPublishable"/>.
/// </summary>
public interface IDiscordOption : ICommandOption
{
    /// <summary>
    /// Converts the option to an instance of <see cref="DiscordApplicationCommandOption"/>
    /// </summary>
    /// <returns>An instance of <see cref="DiscordApplicationCommandOption"/></returns>
    [PublicAPI]
    IEnumerable<DiscordApplicationCommandOption> CreateOptions();
}