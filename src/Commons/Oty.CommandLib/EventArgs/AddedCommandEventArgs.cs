namespace Oty.CommandLib.EventArgs;

/// <summary>
/// Represents arguments for event <see cref="OtyCommandsExtension.CommandAdded"/>.
/// </summary>
[PublicAPI]
public sealed class AddedCommandEventArgs : CommandEventArgs
{
    internal AddedCommandEventArgs(BaseCommandMetadata addedCommand, IMetadataProvider? metadataProvider) : base(metadataProvider)
    {
        this.AddedCommand = addedCommand;
    }

    /// <summary>
    /// Gets the new command added to the extension.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata AddedCommand { get; }
}