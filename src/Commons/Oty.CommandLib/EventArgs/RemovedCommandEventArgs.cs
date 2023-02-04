namespace Oty.CommandLib.EventArgs;

/// <summary>
/// Represents arguments for event <see cref="OtyCommandsExtension.CommandRemoved"/>.
/// </summary>
[PublicAPI]
public sealed class RemovedCommandEventArgs : AsyncEventArgs
{
    internal RemovedCommandEventArgs(BaseCommandMetadata removedCommand)
    {
        this.RemovedCommand = removedCommand;
    }

    /// <summary>
    /// Gets the removed command from the extension.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata RemovedCommand { get; }
}