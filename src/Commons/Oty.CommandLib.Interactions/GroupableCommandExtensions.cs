namespace Oty.CommandLib.Interactions.Utilities;

/// <summary>
/// Represents utility class for <see cref="IGroupableMetadata{TCommand}"/>.
/// </summary>
public static class GroupableCommandExtensions
{
    /// <summary>
    /// Gets subgroups from matched names.
    /// </summary>
    /// <param name="groupableCommand">Groupable command.</param>
    /// <param name="groupNames">Names of the subgroups.</param>
    /// <typeparam name="TCommand">Type of the command</typeparam>
    /// <returns>An instance of all of the <paramref name="groupNames"/> matches, otherwise <see langword="null"/>.</returns>
    public static TCommand? GetCommandFromGroup<TCommand>(this IGroupableMetadata<TCommand> groupableCommand, params string?[]? groupNames)
        where TCommand : BaseCommandMetadata
    {
        IGroupableMetadata<TCommand> subGroupableCommand = groupableCommand;

        for (int i = 0; i < groupNames?.Length; i++)
        {
            var groupInfo = subGroupableCommand?.Groups?
                .OfType<IDiscordGroup<TCommand>>()
                .FirstOrDefault(k => k.Name == groupNames[i]);

            if (groupInfo == null || groupInfo.Subcommand is not IGroupableMetadata<TCommand> foundGroupableCommand)
            {
                return null;
            }

            subGroupableCommand = foundGroupableCommand;
        }

        return subGroupableCommand as TCommand;
    }

    /// <summary>
    /// Gets subgroups from matched names.
    /// </summary>
    /// <param name="groupableCommand">Groupable command.</param>
    /// <param name="groupNames">Names of the subgroups.</param>
    /// <typeparam name="TCommand">Type of the command</typeparam>
    /// <returns>An instance of all of the <paramref name="groupNames"/> matches, otherwise <see langword="null"/>.</returns>
    public static TCommand? GetCommandFromGroup<TCommand>(this IGroupableMetadata<TCommand> groupableCommand, IEnumerable<string?> groupNames)
        where TCommand : BaseCommandMetadata, IDiscordPublishable, IGroupableMetadata<TCommand>, ICommandArgumentsAvaliable<IDiscordOption>
    {
        ArgumentNullException.ThrowIfNull(groupNames, nameof(groupNames));

        return groupableCommand.GetCommandFromGroup(groupNames?.ToArray());
    }

    /// <summary>
    /// Converts groupable methods to suitable <see cref="DiscordApplicationCommandOption"/>.
    /// </summary>
    /// <param name="subgroups">Subgroups to convert.</param>
    /// <typeparam name="TCommand">Type of the command.</typeparam>
    /// <returns>An instance <see cref="DiscordApplicationCommandOption"/></returns>
    public static IEnumerable<DiscordApplicationCommandOption> ConvertToDiscordOptions<TCommand>(this IEnumerable<IGroup<TCommand>> subgroups)
        where TCommand : BaseCommandMetadata, IDiscordPublishable, IGroupableMetadata<TCommand>, ICommandArgumentsAvaliable<IDiscordOption>
    {
        foreach (var subgroup in subgroups.OfType<IDiscordGroup<TCommand>>())
        {
            var subOptions = subgroup.Subcommand.Groups?.Count > 0
                ? subgroup.Subcommand.Groups.OfType<IDiscordGroup<TCommand>>().Select(sc => new DiscordApplicationCommandOption(sc.Name, sc.Description, ApplicationCommandOptionType.SubCommand, null, null, sc.Subcommand.Options?.SelectMany(o => o.CreateOptions()),
                    name_localizations: sc.Localizations.GetNameLocalizations(), description_localizations: sc.Localizations.GetDescriptionLocalizations())).ToList()
                : subgroup.Subcommand.Options?.SelectMany(o => o.CreateOptions()).ToList();

            var commandType = subOptions?.Count != 0 && subOptions?.All(c => c.Type == ApplicationCommandOptionType.SubCommand) == true
                ? ApplicationCommandOptionType.SubCommandGroup
                : ApplicationCommandOptionType.SubCommand;

            yield return new DiscordApplicationCommandOption(subgroup.Name, subgroup.Description, commandType, null, null, subOptions,
                name_localizations: subgroup.Localizations.GetNameLocalizations(), description_localizations: subgroup.Localizations.GetDescriptionLocalizations());
        }
    }
}