namespace Oty.CommandLib.Interactions;

/// <summary>
/// Provides choices for the application command option.
/// </summary>
[PublicAPI]
public interface IChoiceProvider
{
    /// <summary>
    /// Gets choices that for the application command option.
    /// </summary>
    [PublicAPI]
    IEnumerable<DiscordApplicationCommandOptionChoice> GetChoices();
}