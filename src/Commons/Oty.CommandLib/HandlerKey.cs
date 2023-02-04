namespace Oty.CommandLib;

/// <summary>
/// Gets the key that is can be access specified handler and receiver.
/// </summary>
[PublicAPI]
public readonly struct HandlerKey : IEquatable<HandlerKey>
{
    internal HandlerKey(Type eventType, Type applicationCommandType, Type contextType)
    {
        this.EventType = eventType;
        this.CommandType = applicationCommandType;
        this.ContextType = contextType;
    }

    /// <summary>
    /// Gets the type of the event.
    /// </summary>
    [PublicAPI]
    public Type EventType { get; }

    /// <summary>
    /// Gets the type of the <see cref="Entities.BaseCommandMetadata"/> instance.
    /// </summary>
    [PublicAPI]
    public Type CommandType { get; }

    [PublicAPI]
    public Type ContextType { get; }    

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is HandlerKey handlerKey && this.Equals(handlerKey);
    }

    /// <inheritdoc/>
    public bool Equals(HandlerKey obj)
    {
        return this.EventType == obj.EventType &&
            this.CommandType == obj.CommandType;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.EventType, this.CommandType);
    }

    public static bool operator ==(HandlerKey left, HandlerKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HandlerKey left, HandlerKey right)
    {
        return !(left == right);
    }
}