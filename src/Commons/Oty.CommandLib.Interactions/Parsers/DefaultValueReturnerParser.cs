namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A parser class to return default value if there's no value to convert.
/// </summary>
public sealed class DefaultValueReturnerParser : IParameterTypeParser
{
    private static readonly Lazy<DefaultValueReturnerParser> LazyInstance = new(() => new());

    private DefaultValueReturnerParser()
    {
    }

    /// <summary>
    /// Singleton instance of the <see cref="DefaultValueReturnerParser"/>.
    /// </summary>
    public static DefaultValueReturnerParser Instance => LazyInstance.Value;

    /// <inheritdoc/>
    public Type? EventType => null;

    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext parserContext)
    {
        return parserContext.Option.DefaultValue.HasValue && parserContext.Values == null;
    }

    /// <inheritdoc/>
    public Task<Optional<object?>> ConvertValueAsync(ITypeParserContext parserContext)
        => Task.FromResult((parserContext.Option.DefaultValue));
}