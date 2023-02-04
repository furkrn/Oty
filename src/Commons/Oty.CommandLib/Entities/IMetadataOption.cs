namespace Oty.CommandLib.Entities;

/// <summary>
/// Represents backing options from received events.
/// </summary>
public interface IMetadataOption
{
    /// <summary>
    /// Gets the name of the metadata option.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the position of the metadata on the target option.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets the target option of the metadata.
    /// </summary>
    ICommandOption TargetOption { get; }
}