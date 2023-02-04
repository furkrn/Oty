namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A type parser to resolve <see cref="SnowflakeObject"/> from <see cref="ulong"/> that is received from interactions.
/// </summary>
public sealed class DefaultDiscordMentionableResolveInteractionParser : IParameterTypeParser<SnowflakeObject, InteractionCreateEventArgs>
{
    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        return parserContext.Option is IDiscordOption discordOption && discordOption.Type == typeof(SnowflakeObject) &&
            ((IDiscordMetadataOption)discordOption.MetadataOptions[0]).OptionType == ApplicationCommandOptionType.Mentionable;
    }

    /// <inheritdoc/>
    public Task<Optional<SnowflakeObject?>> ConvertValueAsync(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        if (parserContext.Values[0] is not ulong id)
        {
            return Task.FromResult(Optional.FromNoValue<SnowflakeObject?>());
        }

        var snowflakeResult = id switch
        {
            _ when parserContext.EventArgs.Interaction?.Data?.Resolved?.Users?.TryGetValue(id, out var resolvedUser) == true => Optional.FromValue<SnowflakeObject?>(resolvedUser),
            _ when parserContext.EventArgs.Interaction?.Data?.Resolved?.Channels?.TryGetValue(id, out var resolvedChannel) == true => Optional.FromValue<SnowflakeObject?>(resolvedChannel),
            _ when parserContext.EventArgs.Interaction?.Data?.Resolved?.Roles?.TryGetValue(id, out var resolvedUser) == true => Optional.FromValue<SnowflakeObject?>(resolvedUser),
            _ => Optional.FromNoValue<SnowflakeObject?>(),
        };

        return Task.FromResult(snowflakeResult);
    }

    // ReSharper restore all
}