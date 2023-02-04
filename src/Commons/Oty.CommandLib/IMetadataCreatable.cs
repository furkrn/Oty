namespace Oty.CommandLib;

/// <summary>
/// Defines the command module is suitable for creating metadata.
/// </summary>
[PublicAPI]
public interface IMetadataCreatable
{
    /// <summary>
    /// Creates the metadata for the command.
    /// </summary>
    /// <param name="metadataProvider">Collection of configurations and registered services</param>
    [PublicAPI]
    static abstract BaseCommandMetadata CreateMetadata(IMetadataProvider metadataProvider);
}