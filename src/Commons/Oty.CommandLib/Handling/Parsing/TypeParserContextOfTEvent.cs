namespace Oty.CommandLib.Handling;

/// <summary>
/// Represents a event based type parser context.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
[PublicAPI]
public sealed class TypeParserContext<TEvent> : TypeParserContext, ITypeParserContext<TEvent>
    where TEvent : DiscordEventArgs
{
    internal TypeParserContext(DiscordClient client, ICommandOption option, object?[] values, TEvent eventArgs) : base(client, option, values)
    {
        this.EventArgs = eventArgs;
    }

    /// <inheritdoc/>
    [PublicAPI]
    public TEvent EventArgs { get; internal init; }
}