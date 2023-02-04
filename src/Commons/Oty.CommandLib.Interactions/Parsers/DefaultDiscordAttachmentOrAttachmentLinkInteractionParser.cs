namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A type parser to resolve <see cref="DiscordAttachment"/> from see <see cref="ulong"/> that is received from interactions.
/// </summary>
public sealed class DefaultDiscordAttachmentOrAttachmentLinkInteractionParser : IParameterTypeParser<DiscordAttachment, InteractionCreateEventArgs>
{
    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        return parserContext.Option is IDiscordOption discordOption && discordOption.Type == typeof(DiscordAttachment) && 
            ((IDiscordMetadataOption)discordOption.MetadataOptions[0]).OptionType == ApplicationCommandOptionType.Attachment;
    }

    /// <inheritdoc/>
    public Task<Optional<DiscordAttachment?>> ConvertValueAsync(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        if (parserContext.Values[0] is not ulong id)
        {
            return Task.FromResult(Optional.FromNoValue<DiscordAttachment?>());
        }

        if (parserContext.EventArgs.Interaction?.Data?.Resolved?.Attachments.TryGetValue(id, out var attachment) == true)
        {
            return Task.FromResult(Optional.FromValue<DiscordAttachment?>(attachment));
        }
        else
        {
            return Task.FromResult(Optional.FromNoValue<DiscordAttachment?>());
        }

    }
}