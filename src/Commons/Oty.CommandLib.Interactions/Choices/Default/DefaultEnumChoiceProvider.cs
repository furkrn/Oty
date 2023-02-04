namespace Oty.CommandLib.Interactions;

/// <summary>
/// Default <see langword="enum"/> choice provider based on <see cref="string"/>.
/// </summary>
[PublicAPI]
public sealed class DefaultEnumChoiceProvider : IChoiceProvider
{
    private readonly Dictionary<string, object> _choices = new();

    // resharper disable HeuristicUnreachableCode
    /// <summary>
    /// Creates an instance of <see cref="DefaultEnumChoiceProvider"/>.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumType"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="enumType"/> isn't an enum type.</exception>
    public DefaultEnumChoiceProvider(Type enumType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Must be a valid enum type.");
        }

        var enumMembers = Enum.GetValues(enumType);

        foreach (var enumMember in enumMembers)
        {
            var enumMemberInfo = enumType.GetMember(enumMember.ToString()!)[0];

            string? choice = enumMemberInfo.GetCustomAttribute<EnumChoiceAttribute>()?.Name;

            if (enumType.GetCustomAttributes<IgnoreEnumMemberAttributesAttribute>() == null)
            {
                choice ??= enumMemberInfo.GetCustomAttribute<EnumMemberAttribute>()?.Value;
            }

            choice ??= enumMember.ToString()!;

            this._choices.Add(choice, enumMember);
        }
    }

    // resharper restore all

    /// <inheritdoc/>
    [PublicAPI]
    public IEnumerable<DiscordApplicationCommandOptionChoice> GetChoices()
        => this._choices.Select(choice => new DiscordApplicationCommandOptionChoice(choice.Key, choice.Value.ToString()!));
}