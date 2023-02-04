namespace Oty.CommandLib.Interactions.Entities;

/// <summary>
/// Class to provide maximum and minimum integer/double values for application command option
/// </summary>
public record struct DiscordOptionRange
{
    /// <summary>
    /// Creates class to provide maximum and minimum double or integer values for the application command option.
    /// </summary>
    /// <param name="minimum">Minimum value for the option</param>
    /// <param name="maximum">Maximum value for the option</param>
    public DiscordOptionRange(object? minimum, object? maximum)
    {
        if (minimum is not null and not (double or long))
        {
            throw new ArgumentException($"Value must be long or double : {minimum.GetType()}", nameof(minimum));
        }

        this.MinimumValue = minimum;

        if (maximum is not null and not (double or long))
        {
            throw new ArgumentException("Value must be long or double", nameof(maximum));
        }

        this.MaximumValue = maximum;
    }

    /// <summary>
    /// Gets the Minmum Value of specified option.
    /// </summary>
    [PublicAPI]
    public object? MinimumValue { get; }

    /// <summary>
    /// Gets the Maximum Value of specified option.
    /// </summary>
    [PublicAPI]
    public object? MaximumValue { get; }
}