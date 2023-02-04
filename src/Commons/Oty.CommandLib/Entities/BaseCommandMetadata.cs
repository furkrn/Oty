namespace Oty.CommandLib.Entities;

/// <summary>
/// Represents a command type.
/// </summary>
public abstract class BaseCommandMetadata : IEquatable<BaseCommandMetadata>
{
    protected BaseCommandMetadata(string name, Type commandType)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be whitespace or null.", nameof(name));
        }

        if (!typeof(BaseCommandModule).IsAssignableFrom(commandType))
        {
            throw new ArgumentException($"{commandType.Name} must be inherited from {nameof(BaseCommandModule)}.");
        }

        this.Name = name;
        this.ModuleType = commandType;
    }

    /// <summary>
    /// Gets the Name of the application command.
    /// </summary>
    [PublicAPI]
    public string Name { get; protected internal init; }

    /// <summary>
    /// Gets the specified type of the <see cref="BaseCommandModule"/>.
    /// </summary>
    /// <value></value>
    [PublicAPI]
    public Type ModuleType { get; protected internal init; }

    public override bool Equals(object? obj)
        => obj is BaseCommandMetadata other && this.Equals(other);

    public virtual bool Equals(BaseCommandMetadata? other)
        => other is not null &&
            this.Name == other.Name &&
            this.ModuleType == other.ModuleType;

    public override int GetHashCode()
        => HashCode.Combine(this.Name, this.ModuleType);

    public static bool operator ==(BaseCommandMetadata left, BaseCommandMetadata right)
        => left.Equals(right);

    public static bool operator !=(BaseCommandMetadata left, BaseCommandMetadata right)
        => !(left == right);
}