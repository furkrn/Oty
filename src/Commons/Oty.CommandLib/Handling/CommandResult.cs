namespace Oty.CommandLib.Handling;

/// <summary>
/// Represents the execution result of a command.
/// </summary>
[PublicAPI]
public readonly struct CommandResult : IEquatable<CommandResult>
{
    private CommandResult(BaseCommandMetadata metadata, DiscordEventArgs eventArgs, bool isExecuted, Exception? exception = null)
    {
        this.ExecutedCommand = metadata;
        this.Event = eventArgs;
        this.IsExecuted = isExecuted;
        this.Exception = exception;
    }

    /// <summary>
    /// Gets the thrown exception from execution of the command.
    /// </summary>
    [PublicAPI]
    public Exception? Exception { get; }

    /// <summary>
    /// Gets whether command is executed successfully or not.
    /// </summary>
    [PublicAPI]
    public bool IsExecuted { get; }

    /// <summary>
    /// Gets the executed command.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata ExecutedCommand { get; }

    /// <summary>
    /// Gwts received event related to the command.
    /// </summary>
    [PublicAPI]
    public DiscordEventArgs Event { get; }

    /// <summary>
    /// Creates a command result for successfully executed commands.
    /// </summary>
    /// <param name="metadata">Executed command's metadata</param>
    /// <param name="eventArgs"></param>
    /// <returns>An instance of <see cref="CommandResult"/></returns>
    [PublicAPI]
    public static CommandResult FromSuccess(BaseCommandMetadata metadata, DiscordEventArgs eventArgs)
        => new(metadata, eventArgs, true);

    /// <summary>
    /// Creates a command result for unsuccessfull execution of a command.
    /// </summary>
    /// <param name="metadata">Failed command's metadata.</param>
    /// <param name="eventArgs">Related event</param>
    /// <param name="exception">Thrown exception</param>
    /// <returns>An instance of <see cref="CommandResult"/></returns>
    [PublicAPI]
    public static CommandResult FromFail(BaseCommandMetadata metadata, DiscordEventArgs eventArgs, Exception? exception)
        => new(metadata, eventArgs, false, exception);

    /// <inheritdoc/>
    public bool Equals(CommandResult other)
    {
        return this.Exception == other.Exception &&
            this.IsExecuted == other.IsExecuted &&
            this.ExecutedCommand == other.ExecutedCommand &&
            this.Event == other.Event;
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is CommandResult other && this.Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.IsExecuted, this.ExecutedCommand, this.Event);
    }

    public static bool operator ==(CommandResult left, CommandResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CommandResult left, CommandResult right)
    {
        return !(left == right);
    }
}