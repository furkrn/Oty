namespace Oty.CommandLib.Handling;
/// <summary>
/// Provides capabilites converting specified value to what option wants.
/// </summary>
[PublicAPI]
public interface IParameterTypeParser
{
    /// <summary>
    /// Gets the event type that is required to get to convert argument.
    /// </summary>
    [PublicAPI]
    Type? EventType { get; }

    /// <summary>
    /// Checks if the parser is capable converting of the specified type or value or option.
    /// </summary>
    /// <param name="parserContext">Context of the converter.</param>
    /// <returns><see langword="true"/> if the parser can convert, <see langword="false"/> if it cannot convert</returns>
    [PublicAPI]
    bool CanConvert(ITypeParserContext parserContext);

    /// <summary>
    /// Converts received value to of an instance.
    /// </summary>
    /// <returns>Returns a wrapper of an instance that may have value or not.</returns>
    [PublicAPI]
    Task<Optional<object?>> ConvertValueAsync(ITypeParserContext parserContext);
}

/// <summary>
/// Provides capabilites converting specified value to what option wants.
/// </summary>
/// <typeparam name="T">Type to convert.</typeparam>
[PublicAPI]
public interface IParameterTypeParser<T> : IParameterTypeParser
{
    Type? IParameterTypeParser.EventType => null;

    [PublicAPI]
    async Task<Optional<object?>> IParameterTypeParser.ConvertValueAsync(ITypeParserContext parserContext)
    {
        var result = await this.ConvertValueAsync(parserContext).ConfigureAwait(false);

        if (!result.HasValue)
        {
            return Optional.FromNoValue<object?>();
        }

        return Optional.FromValue<object?>(result.Value);
    }

    /// <summary>
    /// Converts received value to a <typeparamref name="T"/> instance.
    /// </summary>
    /// <param name="parserContext"></param>
    /// <returns>Returns a wrapper of <typeparamref name="T"/> that may have value or not.</returns>
    [PublicAPI]
    new Task<Optional<T?>> ConvertValueAsync(ITypeParserContext parserContext);
}
/// <summary>
/// Provides capabilites converting specified value to what option wants using event that is received from Discord.
/// </summary>
/// <typeparam name="T">Type to convert</typeparam>
/// <typeparam name="TEvent">Type of the event received from Discord.</typeparam>
public interface IParameterTypeParser<T, in TEvent> : IParameterTypeParser<T>
    where TEvent : DiscordEventArgs
{
    Type IParameterTypeParser.EventType => typeof(TEvent);

    bool IParameterTypeParser.CanConvert(ITypeParserContext parserContext)
    {
        return parserContext is ITypeParserContext<TEvent> eventParserContext && this.CanConvert(eventParserContext);
    }

    /// <summary>
    /// Checks if the parser is capable converting of the specified type or value or option.
    /// </summary>
    [PublicAPI]
    bool CanConvert(ITypeParserContext<TEvent> parserContext);

    [PublicAPI]
    async Task<Optional<T?>> IParameterTypeParser<T>.ConvertValueAsync(ITypeParserContext parserContext)
    {
        if (parserContext is not ITypeParserContext<TEvent> eventParserContext)
        {
            return Optional.FromNoValue<T?>();
        }

        return await this.ConvertValueAsync(eventParserContext).ConfigureAwait(false);
    }

    /// <inheritdoc cref="IParameterTypeParser{T}.ConvertValueAsync(ITypeParserContext)"/>
    Task<Optional<T?>> ConvertValueAsync(ITypeParserContext<TEvent> parserContext);
}

/// <summary>
/// Provides a factory implementention that creates instance of an any <see cref="IParameterTypeParser"/> implementention.
/// </summary>
[PublicAPI]
public interface IParameterTypeParserFactory : IParameterTypeParser
{
    Type? IParameterTypeParser.EventType => null;

    async Task<Optional<object?>> IParameterTypeParser.ConvertValueAsync(ITypeParserContext parserContext)
    {
        var parser = this.CreateParser(parserContext);

        if (parser is IParameterTypeParserFactory)
        {
            throw new InvalidOperationException("Factory cannot be used to create instance of an another factory!");
        }

        return await (parser?.ConvertValueAsync(parserContext) ?? Task.FromResult(Optional.FromNoValue<object?>())).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a instance of a parser to convert/resolve value.
    /// </summary>
    [PublicAPI]
    IParameterTypeParser? CreateParser(ITypeParserContext parserContext);
}