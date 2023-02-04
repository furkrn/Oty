namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Encapsulates information about a property for monitoring it.
/// </summary>
[PublicAPI]
public class PropertyWatch : IEquatable<PropertyWatch>
{
    internal PropertyWatch(Type optionsType, PropertyInfo propertyInfo)
    {
        this.OptionsType = optionsType ?? throw new ArgumentNullException(nameof(optionsType));
        this.PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
    }

    /// <summary>
    /// Gets the type of the option that contains the property.
    /// </summary>
    [PublicAPI]
    public Type OptionsType { get; }

    /// <summary>
    /// Gets the target property.
    /// </summary>
    /// <value></value>
    [PublicAPI]
    public PropertyInfo PropertyInfo { get; }

    /// <inheritdoc/>
    public bool Equals(PropertyWatch? other)
    {
        return other is not null &&
            this.OptionsType == other.OptionsType &&
            this.PropertyInfo == other.PropertyInfo;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is PropertyWatch other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(this.OptionsType, this.PropertyInfo);

    public static bool operator ==(PropertyWatch left, PropertyWatch right)
        => left.Equals(right);

    public static bool operator !=(PropertyWatch left, PropertyWatch right)
        => !(left == right);
}