namespace Oty.CommandLib.Interactions.Parsers;

/// <summary>
/// A type parser to resolve <see cref="DiscordRole"/> from <see cref="ulong"/> that is received from interactions.
/// </summary>
public sealed class DefaultDiscordRoleResolveParser : IParameterTypeParser<DiscordRole, InteractionCreateEventArgs>
{
    /// <inheritdoc/>
    public bool CanConvert(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        return parserContext.Option is IDiscordOption discordOption && discordOption.Type == typeof(DiscordRole) &&
            ((IDiscordMetadataOption)discordOption.MetadataOptions[0]).OptionType == ApplicationCommandOptionType.Role;
    }

    /// <inheritdoc/>
    public Task<Optional<DiscordRole?>> ConvertValueAsync(ITypeParserContext<InteractionCreateEventArgs> parserContext)
    {
        if (parserContext.Values[0] is not ulong roleId)
        {
            return Task.FromResult(Optional.FromNoValue<DiscordRole?>());
        }

        if (parserContext.EventArgs.Interaction?.Data?.Resolved?.Roles?.TryGetValue(roleId, out var role) == true)
        {
            return Task.FromResult(Optional.FromValue<DiscordRole?>(role));
        }

        return Task.FromResult(Optional.FromNoValue<DiscordRole?>());
    }
}