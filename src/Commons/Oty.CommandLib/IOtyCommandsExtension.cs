namespace Oty.CommandLib;

/// <summary>
/// Provides command extension for handling commands.
/// </summary>
[PublicAPI]
public interface IOtyCommandsExtension : IDisposable
{
    /// <summary>
    /// Gets client.
    /// </summary>
    [PublicAPI]
    DiscordClient Client { get; }

    /// <summary>
    /// Gets the registered commands.
    /// </summary>
    [PublicAPI]
    IReadOnlyCollection<BaseCommandMetadata> RegisteredCommands { get; }

    /// <summary>
    /// Gets the registered handlers to this command.
    /// </summary>
    [PublicAPI]
    IReadOnlyDictionary<HandlerKey, ICommandMetadataReceiver> Receivers { get; }

    /// <summary>
    /// Gets the command parser.
    /// </summary>
    [PublicAPI]
    ICommandParser CommandParser { get; }

    /// <summary>
    /// Fired when any command is handled.
    /// </summary>
    [PublicAPI]
    event AsyncEventHandler<IOtyCommandsExtension, CommandHandledEventArgs> CommandHandled;

    /// <summary>
    /// Fired when a new command added to the extension.
    /// </summary>
    [PublicAPI]
    event AsyncEventHandler<IOtyCommandsExtension, AddedCommandEventArgs> CommandAdded;

    /// <summary>
    /// Fired when older command is requested to be updated.
    /// </summary>
    [PublicAPI]
    event AsyncEventHandler<IOtyCommandsExtension, UpdatedCommandEventArgs> CommandUpdated;

    /// <summary>
    /// Fired when a existing command is removed from the extension.
    /// </summary>
    [PublicAPI]
    event AsyncEventHandler<IOtyCommandsExtension, RemovedCommandEventArgs> CommandRemoved;

    /// <summary>
    /// Adds or updates command's handler and it's receiver to the extensions.
    /// </summary>
    /// <param name="receiver">Receiver of an command.</param>
    /// <typeparam name="TEvent">Type of the event to receive.</typeparam>
    /// <typeparam name="TCommand">Type of the command to find and execute.</typeparam>
    /// <typeparam name="TContext">Type of the command context to create/receive.</typeparam>
    [PublicAPI]
    bool AddReceiver<TEvent, TCommand, TContext>(ICommandMetadataReceiver<TEvent, TCommand, TContext> receiver)
        where TEvent : DiscordEventArgs
        where TCommand : BaseCommandMetadata
        where TContext : BaseCommandContext;

    /// <summary>
    /// Removes an handler and receiver from extension with its key.
    /// </summary>
    /// <param name="key">Key of the receiver and handler to remove.</param>
    /// <returns><see langword="true"/> if it's removed successfully, otherwise <see langword="false"/>.</returns>
    [PublicAPI]
    bool RemoveHandler(HandlerKey key);

    /// <summary>
    /// Registers command to the extension.
    /// </summary>
    /// <typeparam name="TCommand">Command module to register.</typeparam>
    /// <param name="metadataProvider">The metadata provider for the command metadata.</param>
    /// <exception cref="NotSupportedException">Thrown when specified command can't be executed by this type of <see cref="ICommandExecutor"/></exception>
    /// <exception cref="InvalidOperationException">Thrown when specified <typeparamref name="TCommand"/> returned <see langword="null"/> <see cref="BaseCommandMetadata"/></exception>
    /// <returns>Returns <see langword="true"/> when command is registered successfully, otherwise <see langword="false"/>.</returns>
    [PublicAPI]
    Task<bool> RegisterCommandAsync<TCommand>(IMetadataProvider? metadataProvider = null)
        where TCommand : BaseCommandModule, IMetadataCreatable;

    /// <summary>
    /// Registers command to the extension.
    /// </summary>
    /// <param name="commandType">The type of the command module.</param>
    /// <param name="metadataProvider">The metadata provider for the command metadata.</param>
    /// <exception cref="ArgumentException">Thrown when specified type doesn't inherit <see cref="BaseCommandMetadata"/> and <see cref="IMetadataCreatable"/>. </exception>
    /// <exception cref="NotSupportedException">Thrown when specified command can't be executed by this type of <see cref="ICommandExecutor"/></exception>
    /// <exception cref="InvalidOperationException">Thrown when specified <paramref name="commandType"/> returned <see langword="null"/> <see cref="BaseCommandMetadata"/></exception>
    /// <returns>Returns <see langword="true"/> when command is registered successfully, the same command was register <see langword="false"/>.</returns>
    Task<bool> RegisterCommandAsync(Type commandType, IMetadataProvider? metadataProvider = null);

    /// <summary>
    /// Updates the existing command to a newer one.
    /// </summary>
    /// <param name="oldCommandMetadata">The existing command to replace.</param>
    /// <param name="newMetadataProvider">The metadata provider for the command metadata.</param>
    /// <typeparam name="TCommand">Command module that replaces <paramref name="oldCommandMetadata"/></typeparam>
    /// <exception cref="NotSupportedException">Thrown when specified command can't be executed by this type of <see cref="ICommandExecutor"/></exception>
    /// <exception cref="InvalidOperationException">Thrown when specified <typeparamref name="TCommand"/> returned <see langword="null"/> <see cref="BaseCommandMetadata"/></exception>
    /// <returns>Returns <see langword="true"/> when command is updated successfully, otherwise <see langword="false"/>.</returns>
    [PublicAPI]
    Task<bool> UpdateCommandAsync<TCommand>(BaseCommandMetadata oldCommandMetadata, IMetadataProvider? newMetadataProvider = null)
        where TCommand : BaseCommandModule, IMetadataCreatable;

    /// <summary>
    /// Updates the existing command to a newer one.
    /// </summary>
    /// <param name="oldCommandMetadata">Existing command to replace.</param>
    /// <param name="commandType">Command module that replaces <paramref name="oldCommandMetadata"/></param>
    /// <param name="newMetadataProvider">The metadata provider for the command metadata.</param>
    /// <exception cref="ArgumentException">Thrown when specified type doesn't inherit <see cref="BaseCommandMetadata"/> and <see cref="IMetadataCreatable"/>. </exception>
    /// <exception cref="NotSupportedException">Thrown when specified command can't be executed by this type of <see cref="ICommandExecutor"/></exception>
    /// <exception cref="InvalidOperationException">Thrown when specified <paramref name="commandType"/>  returned <see langword="null"/> <see cref="BaseCommandMetadata"/></exception>
    /// <returns>Returns <see langword="true"/> when command is updated successfully, otherwise <see langword="false"/>.</returns>
    Task<bool> UpdateCommandAsync(BaseCommandMetadata oldCommandMetadata, Type commandType, IMetadataProvider? newMetadataProvider = null);

    /// <summary>
    /// Removes command from the extension.
    /// </summary>
    /// <param name="command">Command to remove.</param>
    /// <returns>Returns <see langword="true"/> when command is removed successfully, otherwise <see langword="false"/>.</returns>
    [PublicAPI]
    Task<bool> UnregisterCommandAsync(BaseCommandMetadata command);
}