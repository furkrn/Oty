namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A type parser to resolve <see cref="DiscordChannel"/> from <see cref="ulong"/> that is received from interactions.
/// </summary>
public sealed class DefaultDiscordChannelInteractionParser : IParameterTypeParser<DiscordChannel, InteractionCreateEventArgs>
{
    // ReSharper disable ConstantConditionalAccessQualifier

    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        return parserContext.Option is IDiscordOption discordOption && discordOption.Type == typeof(DiscordChannel) &&
            ((IDiscordMetadataOption)discordOption.MetadataOptions[0]).OptionType == ApplicationCommandOptionType.Channel;
    }

    /// <inheritdoc/>
    public async Task<Optional<DiscordChannel?>> ConvertValueAsync(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        if (parserContext.Values[0] is not ulong channelId)
        {
            return Optional.FromNoValue<DiscordChannel?>();
        }

        if (parserContext.EventArgs.Interaction?.Data?.Resolved?.Channels?.TryGetValue(channelId, out var channel) != true)
        {
            channel = await parserContext.Client.GetChannelAsync(channelId).ConfigureAwait(false);
        }

        return Optional.FromValue(channel);
    }
}