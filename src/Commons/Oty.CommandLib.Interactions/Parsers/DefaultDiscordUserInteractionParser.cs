namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A type parser to resolve <see cref="DiscordUser"/> from <see cref="ulong"/> that is received from interactions.
/// </summary>
public sealed class DefaultDiscordUserInteractionParser : IParameterTypeParser<DiscordUser, InteractionCreateEventArgs>
{
    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        return parserContext.Option is IDiscordOption discordOption && typeof(DiscordUser).IsAssignableFrom(discordOption.Type) && 
            ((IDiscordMetadataOption)discordOption.MetadataOptions[0]).OptionType == ApplicationCommandOptionType.User;
    }

    /// <inheritdoc/>
    public async Task<Optional<DiscordUser?>> ConvertValueAsync(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        if (parserContext.Values[0] is not ulong userId)
        {
            return Optional.FromNoValue<DiscordUser?>();
        }

        if (parserContext.EventArgs.Interaction?.Data.Resolved?.Users?.TryGetValue(userId, out var user) != true)
        {
            user = await parserContext.Client.GetUserAsync(userId).ConfigureAwait(false);
        }

        return Optional.FromValue(user);
    }
}