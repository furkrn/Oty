namespace Oty.Things.AsyncMonitor;

/// <summary>
/// Compared value for the specified options.
/// </summary>
[PublicAPI]
public readonly struct ComparedValue<TOptions, TProperty> : IEquatable<ComparedValue<TOptions, TProperty>>
{
    /// <summary>
    /// Creates an instance of <see cref="ComparedValue{TOptions, TProperty}"/>.
    /// </summary>
    /// <param name="option">The instance of the option.</param>
    /// <param name="oldValue">The old value of the property.</param>
    /// <param name="newValue">The current value of the property</param>
    public ComparedValue(TOptions option, TProperty? oldValue, TProperty? newValue)
    {
        this.Options = option ?? throw new ArgumentNullException(nameof(option));
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }

    /// <summary>
    /// Gets the option.
    /// </summary>
    [PublicAPI]
    public TOptions Options { get; }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    [PublicAPI]
    public TProperty? OldValue { get; }

    /// <summary>
    /// Gets the current value.
    /// </summary>
    [PublicAPI]
    public TProperty? NewValue { get; }
    
    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public bool Equals(ComparedValue<TOptions, TProperty> other)
    {
        return EqualityComparer<TOptions>.Default.Equals(this.Options, other.Options) &&
            EqualityComparer<TProperty>.Default.Equals(this.OldValue, this.NewValue) &&
            EqualityComparer<TProperty>.Default.Equals(this.OldValue, this.NewValue);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Options, this.OldValue, this.NewValue);
    }

    public static bool operator ==(ComparedValue<TOptions, TProperty> left, ComparedValue<TOptions, TProperty> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ComparedValue<TOptions, TProperty> left, ComparedValue<TOptions, TProperty> right)
    {
        return !(left == right);
    }
}