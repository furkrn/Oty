namespace Oty.CommandLib.Entities;

/// <summary>
/// Represents a option of a command.
/// </summary>
public interface ICommandOption
{
    /// <summary>
    /// Gets the name of the option.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type declarition of the option.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the default value of the option if it's avaliable.
    /// </summary>
    Optional<object?> DefaultValue { get; }

    /// <summary>
    /// Gets the position of the option on the command.
    /// </summary>
    /// <value></value>
    int Position { get; }

    /// <summary>
    /// Gets the metadata options of the option.
    /// </summary>
    IReadOnlyList<IMetadataOption> MetadataOptions { get; }
}