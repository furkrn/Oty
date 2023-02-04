namespace Oty.CommandLib.EventArgs;

/// <summary>
/// Provides command eventargs.
/// </summary>
public abstract class CommandEventArgs : AsyncEventArgs
{
    protected CommandEventArgs(IMetadataProvider? metadataProvider)
    {
        this.MetadataProvider = metadataProvider;
    }

    /// <summary>
    /// Gets the used metadata provider to create metadata of the command.
    /// </summary>
    /// <value></value>
    public IMetadataProvider? MetadataProvider { get; }
}
