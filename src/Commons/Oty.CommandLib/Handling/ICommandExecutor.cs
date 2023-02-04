namespace Oty.CommandLib.Handling;

/// <summary>
/// Represents a command executor.
/// </summary>
public interface ICommandExecutor : IDisposable
{
    /// <summary>
    /// Configures the executor with the extension
    /// </summary>
    /// <param name="extension"></param>
    /// <exception cref="InvalidOperationException">Thrown when executor is already configured</exception>
    void Configure(IOtyCommandsExtension extension);

    /// <summary>
    /// Gets whether command is suitable to execute or not.
    /// </summary>
    /// <param name="metadata">Command the check.</param>
    /// <param name="reason">Reason why it's not suitable to execute.</param>
    /// <returns>Returns <see langword="true"/> if command is suitable, otherwise <see langword="false"/> with <paramref name="reason"/></returns>
    bool CanExecute(BaseCommandMetadata metadata, [MaybeNullWhen(false)] out string? reason);

    /// <summary>
    /// Executes a command.
    /// </summary>
    /// <param name="context">Received result from a receiver</param>
    /// <returns>Returns a <see cref="CommandResult"/> which maybe avaliable or not.</returns>
    Task<Optional<CommandResult>> ExecuteAsync(IReceiverResult<DiscordEventArgs, BaseCommandMetadata, BaseCommandContext> context);
}