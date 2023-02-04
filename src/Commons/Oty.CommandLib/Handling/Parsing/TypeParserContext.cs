namespace Oty.CommandLib.Handling;

/// <summary>
/// Represents a context for type parsers.
/// </summary>
[PublicAPI]
public class TypeParserContext : ITypeParserContext
{
    internal TypeParserContext(DiscordClient client, ICommandOption option, object?[] values)
    {
        this.Client = client;
        this.Option = option;
        this.Values = values;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public DiscordClient Client { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public ICommandOption Option { get; }

    /// <inheritdoc/>
    [PublicAPI]
    public object?[] Values { get; }
}