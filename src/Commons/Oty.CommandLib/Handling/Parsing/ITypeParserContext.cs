namespace Oty.CommandLib.Handling;

/// <summary>
/// Defines a type parser context.
/// </summary>
[PublicAPI]
public interface ITypeParserContext
{
    /// <summary>
    /// Gets the option that needs to converted.
    /// </summary>
    ICommandOption Option { get; }

    /// <summary>
    /// Gets the value to convert.
    /// </summary>
    [PublicAPI]
    object?[] Values { get; }

    /// <summary>
    /// Gets the Discord client.
    /// </summary>
    [PublicAPI]
    DiscordClient Client { get; }
}

/// <summary>
/// Defines a event based type parser context.
/// </summary>
/// <typeparam name="TEvent">Type of the event.</typeparam>
[PublicAPI]
public interface ITypeParserContext<out TEvent> : ITypeParserContext
    where TEvent : DiscordEventArgs
{
    /// <summary>
    /// Gets the event that can be use to convert value.
    /// </summary>
    [PublicAPI]
    TEvent EventArgs { get; }
}