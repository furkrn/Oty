namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// TypeParser for <see cref="Enum"/> based on <see cref="string"/>. Converts <see cref="string"/> to specified <see cref="Enum"/> type.
/// </summary>
public sealed class DefaultEnumParser : IParameterTypeParser
{
    public Type? EventType => null;

    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Option.Type?.IsEnum == true && parserContext.Values[0] is string;
    }

    /// <inheritdoc/>
    public Task<Optional<object?>> ConvertValueAsync(ITypeParserContext parserContext)
    {
        if (parserContext.Values[0] is not string rawText)
        {
            return Task.FromResult(Optional.FromNoValue<object?>());
        }

        if (!Enum.TryParse(parserContext.Option.Type!, rawText, out object? result))
        {
            return Task.FromResult(Optional.FromNoValue<object?>());
        }

        return Task.FromResult(Optional.FromValue<object?>(result));
    }
}