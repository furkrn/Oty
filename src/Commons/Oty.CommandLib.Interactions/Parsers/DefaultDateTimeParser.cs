namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// TypeParser for <see cref="DateTime"/> based on <see cref="string"/>. Converts <see cref="string"/> to <see cref="DateTime"/>
/// </summary>
public sealed class DefaultDateTimeParser : IParameterTypeParser<DateTime>
{
    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Option.Type == typeof(DateTime) && parserContext.Values.FirstOrDefault() != null;
    }

    /// <inheritdoc/>
    public Task<Optional<DateTime>> ConvertValueAsync(ITypeParserContext parserContext)
    {
        if (parserContext.Values[0] is not string rawText)
        {
            return Task.FromResult(Optional.FromNoValue<DateTime>());
        }

        if (!DateTime.TryParse(rawText, out var dateTime))
        {
            return Task.FromResult(Optional.FromNoValue<DateTime>());
        }

        return Task.FromResult(Optional.FromValue(dateTime));
    }
}