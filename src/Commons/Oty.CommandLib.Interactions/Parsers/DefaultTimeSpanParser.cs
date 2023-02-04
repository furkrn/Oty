namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// TypeParser for <see cref="TimeSpan"/> based on <see cref="string"/>. Converts <see cref="string"/> to <see cref="DateTime"/>.
/// </summary>
public sealed class DefaultTimeSpanParser : IParameterTypeParser<TimeSpan>
{
    private readonly Regex _parsingRegex = new("^(?:(?<years>[0-9]+)y)?(?:(?<months>[0-9]+)mo)?(?:(?<weeks>[0-9]+)w)?(?:(?<days>[0-9]+)d)?(?:(?<hours>[0-9]+)h)?(?:(?<minutes>[0-9]+)m)?(?:(?<seconds>[0-9]+)s)?$",
        RegexOptions.ECMAScript | RegexOptions.Compiled);

    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Option.Type == typeof(TimeSpan) && parserContext.Values[0] is string;
    }

    /// <inheritdoc/>
    public Task<Optional<TimeSpan>> ConvertValueAsync(ITypeParserContext parserContext)
    {
        if (parserContext.Values[0] is not string rawText)
        {
            return Task.FromResult(Optional.FromNoValue<TimeSpan>());
        }

        var matches = _parsingRegex.Match(rawText);
        if (matches.Length == 0)
        {
            return Task.FromResult(Optional.FromNoValue<TimeSpan>());
        }

        var timers = new Dictionary<string, int>();
        foreach (var group in matches.Groups.Values)
        {
            if (!int.TryParse(group.Value, out int time))
            {
                timers[group.Name] = 0;
                continue;
            }

            timers[group.Name] = time;
        }

        var result = DateTime.Today.AddYears(timers["years"])
            .AddMonths(timers["months"])
            .AddDays((timers["weeks"] * 7) + timers["days"])
            .AddHours(timers["hours"])
            .AddMinutes(timers["minutes"])
            .AddSeconds(timers["seconds"]) - DateTime.Today;

        return Task.FromResult(Optional.FromValue(result));
    }
}