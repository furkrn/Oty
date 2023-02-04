namespace Oty.CommandLib.EventArgs;

/// <summary>
/// Represents arguments for event <see cref="OtyCommandsExtension.CommandUpdated"/>.
/// </summary>
[PublicAPI]
public sealed class UpdatedCommandEventArgs : CommandEventArgs
{
    internal UpdatedCommandEventArgs(BaseCommandMetadata oldCommand, BaseCommandMetadata newCommand, IMetadataProvider? newMetadataProvider) : base(newMetadataProvider)
    {
        this.NewCommand = newCommand;
        this.OldCommand = oldCommand;
    }

    /// <summary>
    /// Gets the command that will replace <see cref="OldCommand"/>.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata NewCommand { get; }

    /// <summary>
    /// Gets the command that is need to be updated.
    /// </summary>
    [PublicAPI]
    public BaseCommandMetadata OldCommand { get; }
}