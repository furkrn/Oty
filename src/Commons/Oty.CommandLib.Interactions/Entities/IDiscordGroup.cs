namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Provides information about group.
/// </summary>
/// <typeparam name="TCommand">Type of the command.</typeparam>
public interface IDiscordGroup<out TCommand> : IGroup<TCommand>
    where TCommand : BaseCommandMetadata
{
    /// <summary>
    /// Gets the name of the group.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the group.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the localizations for this option.
    /// </summary>
    DiscordLocalizations Localizations { get; }
}